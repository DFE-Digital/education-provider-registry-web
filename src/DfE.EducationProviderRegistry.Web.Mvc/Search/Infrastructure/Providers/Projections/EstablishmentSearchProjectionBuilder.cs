using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Providers.Projections;

public sealed class EstablishmentSearchProjectionBuilder
    : ISearchProjectionBuilder<Establishment>
{
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