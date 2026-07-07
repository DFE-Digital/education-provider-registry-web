using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Providers.Projections;

/// <summary>
/// Builds the base <see cref="IQueryable{T}"/> used for establishment search
/// operations. This builder is responsible for returning an EF Core query
/// rooted at <see cref="Establishment"/> with all navigation properties eagerly
/// loaded so that downstream pipeline steps (ordering, mapping, projection)
/// can access related data without additional database queries.
/// </summary>
/// <remarks>
/// This builder does not apply filtering, ordering, or projection. It only
/// constructs the entity graph required by the search pipeline. Higher‑level
/// components (search orchestrator, ordering step, mapping step) are
/// responsible for shaping the results into search‑specific DTOs.
/// </remarks>
[ExcludeFromCodeCoverage]
public sealed class EstablishmentSearchProjectionBuilder
    : ISearchProjectionBuilder<Establishment>
{
    /// <summary>
    /// Constructs the EF Core query used as the root for establishment search.
    /// The returned query:
    /// <list type="bullet">
    /// <item><description>Is <see cref="EntityFrameworkQueryableExtensions.AsNoTracking{TEntity}(IQueryable{TEntity})"/> to avoid change tracking overhead.</description></item>
    /// <item><description>Eagerly loads all navigation properties required by the search pipeline:
    /// <see cref="Establishment.Site"/>,
    /// <see cref="Establishment.EstablishmentType"/>,
    /// <see cref="Establishment.EstablishmentAuthority"/>,
    /// <see cref="Establishment.EstablishmentGroupMembership"/> and its
    /// related <see cref="Group"/> and <see cref="GroupType"/>.</description></item>
    /// <item><description>Returns an <see cref="IQueryable{Establishment}"/> that can be further
    /// composed by the search orchestrator.</description></item>
    /// </list>
    /// </summary>
    /// <param name="db">
    /// The active <see cref="DbContext"/> instance. Must be an
    /// <see cref="EducationProviderRegistryDbContext"/>.
    /// </param>
    /// <returns>
    /// An <see cref="IQueryable{Establishment}"/> with all required navigation
    /// properties eagerly loaded.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="db"/> is <c>null</c>.
    /// </exception>
    public IQueryable<Establishment> Build(DbContext db)
    {
        ArgumentNullException.ThrowIfNull(db);

        EducationProviderRegistryDbContext ctx =
            (EducationProviderRegistryDbContext)db;

        return ctx.Establishment
            .AsNoTracking()
            .Include(establishemnt => establishemnt.Site)
            .Include(establishemnt => establishemnt.EstablishmentType)
            .Include(establishemnt => establishemnt.EstablishmentAuthority)
            .Include(establishemnt => establishemnt.EstablishmentGroupMembership)
                .ThenInclude(groupMembership => groupMembership.Group)
                    .ThenInclude(group => group.GroupType);
    }
}
