using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Providers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Providers;

/// <summary>
/// Provides facet aggregation for <see cref="Establishment"/> search results.
/// This implementation groups establishments by a configured facet selector
/// and returns the distinct facet values along with their occurrence counts.
/// </summary>
/// <remarks>
/// The provider operates over a filtered subset of establishments identified
/// by URNs. It does not perform search, filtering, or ordering logic beyond
/// grouping and counting. Facet selectors must be valid LINQ expressions that
/// EF Core can translate into SQL.
/// </remarks>
public sealed class EstablishmentFacetProvider : IFacetProvider
{
    private readonly IDbContextFactory<EducationProviderRegistryDbContext> _contextFactory;

    /// <summary>
    /// A mapping of facet names to expressions that extract the facet value
    /// from an <see cref="Establishment"/> entity. Keys must match the facet
    /// names requested by the search pipeline.
    /// </summary>
    private readonly Dictionary<string, Expression<Func<Establishment, object>>> _facetSelectors;

    /// <summary>
    /// Creates a new instance of <see cref="EstablishmentFacetProvider"/>.
    /// </summary>
    /// <param name="contextFactory">
    /// A factory used to create <see cref="EducationProviderRegistryDbContext"/>
    /// instances for facet evaluation.
    /// </param>
    /// <param name="facetSelectors">
    /// A dictionary mapping facet names to expressions that select the facet
    /// value from an <see cref="Establishment"/>. These expressions determine
    /// how establishments are grouped when computing facet counts.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="contextFactory"/> or
    /// <paramref name="facetSelectors"/> is <c>null</c>.
    /// </exception>
    public EstablishmentFacetProvider(
        IDbContextFactory<EducationProviderRegistryDbContext> contextFactory,
        Dictionary<string, Expression<Func<Establishment, object>>> facetSelectors)
    {
        _contextFactory = contextFactory;
        _facetSelectors = facetSelectors;
    }

    /// <summary>
    /// Computes facet values for the specified facet name across the given
    /// collection of establishment URNs. Each facet value is returned with
    /// the number of establishments that share that value.
    /// </summary>
    /// <param name="ids">
    /// A list of establishment URNs to include in the facet computation.
    /// </param>
    /// <param name="facetName">
    /// The name of the facet to compute. Must correspond to a configured
    /// facet selector in <see cref="_facetSelectors"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that may be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A read-only list of <see cref="FacetResult"/> objects ordered by
    /// descending count.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the facet name is unknown or when the database context
    /// cannot be created.
    /// </exception>
    public async Task<IReadOnlyList<FacetResult>> GetFacetsAsync(
        IReadOnlyList<string> ids,
        string facetName,
        CancellationToken cancellationToken = default)
    {
        EducationProviderRegistryDbContext? context =
            await _contextFactory.CreateDbContextAsync(cancellationToken) ??
                throw new InvalidOperationException("Failed to create database context.");

        await using (context)
        {
            if (!_facetSelectors.TryGetValue(facetName, out var selector))
            {
                throw new InvalidOperationException($"Unknown facet '{facetName}'.");
            }

            IQueryable<Establishment> filtered =
                context.Establishment.Where(establishment =>
                    ids.Contains(establishment.Urn));

            IQueryable<IGrouping<object, Establishment>> grouped =
                filtered.GroupBy(selector);

            IQueryable<dynamic> sqlProjection =
                grouped.Select(groupedFacet => new
                {
                    groupedFacet.Key,
                    Count = groupedFacet.LongCount()
                });

            List<dynamic> rawFacetResults =
                await sqlProjection.ToListAsync(cancellationToken);

            List<FacetResult> results =
                [.. rawFacetResults
                    .Select(facetResult => new FacetResult(
                        facetResult.Key?.ToString() ?? string.Empty,
                        facetResult.Count
                    ))
                    .OrderByDescending(facet => facet.Count)];

            return results;
        }
    }
}
