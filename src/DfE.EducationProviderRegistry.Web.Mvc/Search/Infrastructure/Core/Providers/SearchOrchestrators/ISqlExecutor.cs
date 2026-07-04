using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers.SearchOrchestrators;

public interface ISqlExecutor<TProjection>
{
    Task<List<object>> ExecuteIdsAsync(
        DbContext db,
        string sql,
        string primaryKeyPropertyName,
        CancellationToken cancellationToken = default);
}
