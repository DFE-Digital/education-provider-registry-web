using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers.SearchOrchestrators.Context;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers.SearchOrchestrators;

public interface ISearchOrchestrator<TProjection> where TProjection : class
{
    Task<IReadOnlyList<TProjection>> ExecuteAsync(
        DbContext db,
        IQueryable<TProjection> baseQuery,
        SearchOrchestratorContext context,
        string searchFilters = "",
        CancellationToken cancellationToken = default);
}

