using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers.SearchOrchestrators;

public sealed class SqlExecutor<TProjection> : ISqlExecutor<TProjection>
    where TProjection : class
{
    public async Task<List<object>> ExecuteIdsAsync(
        DbContext db,
        string sql,
        string primaryKeyPropertyName,
        CancellationToken cancellationToken = default)
    {
        return await db.Set<TProjection>()
            .FromSqlRaw(sql)
            .Select(e => EF.Property<object>(e, primaryKeyPropertyName))
            .ToListAsync(cancellationToken);
    }
}