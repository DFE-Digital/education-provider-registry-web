using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Providers.Projections;

public interface ISearchProjectionBuilder<TProjection>
{
    IQueryable<TProjection> Build(DbContext db);
}