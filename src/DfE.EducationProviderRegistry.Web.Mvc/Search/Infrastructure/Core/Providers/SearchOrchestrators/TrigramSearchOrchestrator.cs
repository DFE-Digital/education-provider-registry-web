using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers.SearchOrchestrators;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers.SearchOrchestrators.Context;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers.SearchOrchestrators.EFMetadataResolver;
using Microsoft.EntityFrameworkCore;

public sealed class TrigramSearchOrchestrator<T> : ISearchOrchestrator<T>
    where T : class
{
    private readonly CachedEntityMetadataResolver<T> _metadataResolver;

    public TrigramSearchOrchestrator(CachedEntityMetadataResolver<T> metadataResolver)
    {
        _metadataResolver = metadataResolver;
    }

    public async Task<IReadOnlyList<T>> ExecuteAsync(
        DbContext db,
        IQueryable<T> baseQuery,
        SearchOrchestratorContext context,
        string searchFilters = "",
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(db);
        ArgumentNullException.ThrowIfNull(context);

        EntityMetadata metadata = _metadataResolver.Resolve(db);

        string searchColumn = context.SearchColumn;

        bool columnExists = metadata.EntityType.GetProperties()
            .Any(p => p.GetColumnName() == searchColumn);

        if (!columnExists)
        {
            throw new InvalidOperationException(
                $"Column '{searchColumn}' does not exist on entity {typeof(T).Name}.");
        }

        string sql =
            $@"
            SELECT t.""{metadata.PrimaryKeyColumn}""
            FROM {metadata.Schema}.""{metadata.TableName}"" t
            WHERE t.""{searchColumn}"" % CAST('{context.SearchTerm}' AS text)
            {searchFilters}
            ORDER BY similarity(t.""{searchColumn}"", CAST('{context.SearchTerm}' AS text)) DESC
            LIMIT {context.PageSize} OFFSET {context.Offset}
            ";

        List<object> ids = await db.Set<T>()
            .FromSqlRaw(sql)
            .Select(e => EF.Property<object>(e, metadata.PrimaryKeyProperty.Name))
            .ToListAsync(cancellationToken);

        return await baseQuery
            .Where(e => ids.Contains(EF.Property<object>(e, metadata.PrimaryKeyProperty.Name)))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
