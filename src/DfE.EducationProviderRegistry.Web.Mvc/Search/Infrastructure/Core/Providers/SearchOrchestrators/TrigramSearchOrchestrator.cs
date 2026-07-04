using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers.SearchOrchestrators;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers.SearchOrchestrators.Context;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers.SearchOrchestrators.EFMetadataResolver;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

/// <summary>
/// Executes a PostgreSQL trigram-based search against an EF Core entity,
/// returning a projected result set based on a LINQ <see cref="IQueryable{TProjection}"/>.
/// </summary>
/// <typeparam name="TProjection">
/// The entity or projection type being queried. Must be a reference type.
/// </typeparam>
public sealed class TrigramSearchOrchestrator<TProjection> : ISearchOrchestrator<TProjection>
    where TProjection : class
{
    /// <summary>
    /// Resolves EF Core metadata such as table name, schema, and primary key
    /// for the entity associated with <typeparamref name="TProjection"/>.
    /// </summary>
    private readonly IEntityMetadataResolver<TProjection> _metadataResolver;

    /// <summary>
    /// Executes raw SQL queries used to retrieve primary key values for trigram matches.
    /// Abstracting SQL execution behind this interface allows unit tests to avoid
    /// invoking <see cref="RelationalQueryableExtensions.FromSqlRaw{TEntity}(IQueryable{TEntity}, string, object[])"/>,
    /// which cannot be mocked and is unsupported by non-relational EF providers.
    /// </summary>
    private readonly ISqlExecutor<TProjection> _sqlExecutor;

    /// <summary>
    /// Creates a new instance of the trigram search orchestrator.
    /// </summary>
    /// <param name="metadataResolver">
    /// The metadata resolver used to obtain table, schema, and primary key information
    /// for the EF Core entity associated with <typeparamref name="TProjection"/>.
    /// </param>
    /// <param name="sqlExecutor">
    /// The SQL executor responsible for running the raw PostgreSQL trigram query.
    /// This abstraction enables pure unit testing by allowing SQL execution to be mocked,
    /// avoiding EF Core provider limitations when using <c>FromSqlRaw</c>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="metadataResolver"/> or <paramref name="sqlExecutor"/> is null.
    /// </exception>
    public TrigramSearchOrchestrator(
        IEntityMetadataResolver<TProjection> metadataResolver,
        ISqlExecutor<TProjection> sqlExecutor)
    {
        _metadataResolver = metadataResolver;
        _sqlExecutor = sqlExecutor;
    }

    /// <summary>
    /// Executes a trigram similarity search using PostgreSQL <c>pg_trgm</c> operators,
    /// returning a filtered and ordered result set based on the provided search context.
    /// </summary>
    /// <param name="dbContext">The EF Core <see cref="DbContext"/> used to execute the query.</param>
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

        EntityMetadata metadata = _metadataResolver.Resolve(dbContext);

        string searchColumn = context.SearchColumn;

        bool columnExists =
            metadata.EntityType
                .GetProperties()
                .Any(property =>
                    property.GetColumnName() == searchColumn);

        if (!columnExists){
            throw new InvalidOperationException(
                $"Column '{searchColumn}' does not exist on entity {typeof(TProjection).Name}.");
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

        List<object> ids =
            await _sqlExecutor.ExecuteIdsAsync(
                dbContext,
                sql,
                metadata.PrimaryKeyProperty.Name,
                cancellationToken);

        return [
            .. baseQuery
                .AsEnumerable()
                .Where(parameter =>
                {
                    if (parameter is null){
                        throw new InvalidOperationException(
                            $"Entity instance is null when evaluating primary key '{metadata.PrimaryKeyProperty.Name}'.");
                    }

                    PropertyInfo? pkProp =
                        parameter.GetType().GetProperty(metadata.PrimaryKeyProperty.Name) ??
                            throw new InvalidOperationException(
                                $"Primary key property '{metadata.PrimaryKeyProperty.Name}' not found on type '{parameter.GetType().Name}'.");

                    object? pkValue = pkProp.GetValue(parameter);

                    return pkValue is null
                        ? throw new InvalidOperationException(
                            $"Primary key value for '{metadata.PrimaryKeyProperty.Name}' is null on entity '{parameter.GetType().Name}'.")
                        : ids.Contains(pkValue);
                })
        ];
    }
}