using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Providers;

public sealed class EstablishmentFacetProvider : IFacetProvider<Establishment>
{
    private readonly IDbContextFactory<EducationProviderRegistryDbContext> _contextFactory;
    private readonly Dictionary<string, Expression<Func<Establishment, object>>> _facetSelectors;

    public EstablishmentFacetProvider(
        IDbContextFactory<EducationProviderRegistryDbContext> contextFactory,
        Dictionary<string, Expression<Func<Establishment, object>>> facetSelectors)
    {
        _contextFactory = contextFactory;
        _facetSelectors = facetSelectors;
    }

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
