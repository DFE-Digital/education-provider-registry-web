using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Providers.SearchOrchestrators.Context;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Providers.SearchOrchestrators.EFMetadataResolver;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Providers.SearchOrchestrators;

/// <summary>
/// Orchestrates a PostgreSQL trigram-based search using <c>pg_trgm</c>,
/// retrieving primary key values via raw SQL and rehydrating full entity or
/// projection instances using a LINQ <see cref="IQueryable{TProjection}"/>.
/// </summary>
/// <typeparam name="TProjection">
/// The entity or projection type being queried. Must be a reference type.
/// </typeparam>
public sealed class TrigramSearchOrchestrator<TProjection> : ISearchOrchestrator<TProjection>
    where TProjection : class
{
    private readonly IEntityMetadataResolver<TProjection> _metadataResolver;
    private readonly ISqlExecutor<TProjection> _sqlExecutor;

    /// <summary>
    /// Creates a new instance of the trigram search orchestrator.
    /// </summary>
    /// <param name="metadataResolver">
    /// Resolves EF Core metadata such as schema, table name, and primary key
    /// for the entity associated with <typeparamref name="TProjection"/>.
    /// </param>
    /// <param name="sqlExecutor">
    /// Executes raw SQL queries used to retrieve primary key values for trigram matches.
    /// Abstracting SQL execution enables pure unit testing without invoking EF Core's
    /// relational <c>FromSqlRaw</c> pipeline.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="metadataResolver"/> or <paramref name="sqlExecutor"/> is null.
    /// </exception>
    public TrigramSearchOrchestrator(
        IEntityMetadataResolver<TProjection> metadataResolver,
        ISqlExecutor<TProjection> sqlExecutor)
    {
        _metadataResolver = metadataResolver
            ?? throw new ArgumentNullException(nameof(metadataResolver));

        _sqlExecutor = sqlExecutor
            ?? throw new ArgumentNullException(nameof(sqlExecutor));
    }

    /// <summary>
    /// Executes a trigram similarity search using PostgreSQL <c>pg_trgm</c> operators,
    /// returning a filtered and ordered result set based on the provided search context.
    /// </summary>
    /// <param name="dbContext">The EF Core <see cref="DbContext"/> used to execute the SQL query.</param>
    /// <param name="baseQuery">
    /// The base LINQ query used to rehydrate full entity or projection instances
    /// after primary key values have been retrieved via raw SQL.
    /// </param>
    /// <param name="context">
    /// The search context containing the search term, paging information,
    /// and the column to apply trigram similarity against.
    /// </param>
    /// <param name="searchFilters">
    /// Optional additional SQL filter clauses appended directly to the trigram query.
    /// Must be valid SQL. Defaults to an empty string.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A read-only list of <typeparamref name="TProjection"/> instances matching the trigram search.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="dbContext"/> or <paramref name="context"/> is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the specified search column does not exist on the entity.
    /// </exception>
    public async Task<IReadOnlyList<TProjection>> ExecuteAsync(
        DbContext dbContext,
        IQueryable<TProjection> baseQuery,
        SearchOrchestratorContext context,
        string searchFilters = "",
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(context);

        var metadata = _metadataResolver.Resolve(dbContext);

        if (!metadata.EntityType
            .GetProperties()
                .Any(property =>
                    property.GetColumnName() == context.SearchColumn))
        {
            throw new InvalidOperationException(
                $"Column '{context.SearchColumn}' does not exist on entity {typeof(TProjection).Name}.");
        }

        // Build trigram SQL
        string sql =
            $@"
            SELECT t.""{metadata.PrimaryKeyColumn}""
            FROM {metadata.Schema}.""{metadata.TableName}"" t
            WHERE t.""{context.SearchColumn}"" % CAST('{context.SearchTerm}' AS text)
            {searchFilters}
            ORDER BY similarity(t.""{context.SearchColumn}"", CAST('{context.SearchTerm}' AS text)) DESC
            LIMIT {context.PageSize} OFFSET {context.Offset}
            ";

        List<object> ids = await _sqlExecutor.ExecuteIdsAsync(
            dbContext,
            sql,
            metadata.PrimaryKeyProperty.Name,
            cancellationToken);

        return [
            .. baseQuery
                .AsEnumerable()
                .Where(projection =>
                    ids.Contains(GetPrimaryKeyValue(
                        projection, metadata.PrimaryKeyProperty.Name)))
            ];
    }

    /// <summary>
    /// Extracts the primary key value from an entity instance using reflection,
    /// with full null-guarding and descriptive exceptions.
    /// </summary>
    /// <param name="entity">The entity instance from which to extract the primary key value.</param>
    /// <param name="pkName">The name of the primary key property.</param>
    /// <returns>The non-null primary key value.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the entity instance is null, the primary key property does not exist,
    /// or the primary key value is null.
    /// </exception>
    private static object GetPrimaryKeyValue(TProjection entity, string pkName)
    {
        if (entity is null)
            throw new InvalidOperationException(
                $"Entity instance is null when evaluating primary key '{pkName}'.");

        var pkProp = entity.GetType().GetProperty(pkName)
            ?? throw new InvalidOperationException(
                $"Primary key property '{pkName}' not found on type '{entity.GetType().Name}'.");

        return pkProp.GetValue(entity)
            ?? throw new InvalidOperationException(
                $"Primary key value for '{pkName}' is null on entity '{entity.GetType().Name}'.");
    }
}
