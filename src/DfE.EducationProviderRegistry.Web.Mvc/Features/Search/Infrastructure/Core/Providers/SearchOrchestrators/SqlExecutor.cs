using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Providers.SearchOrchestrators;

/// <summary>
/// Executes raw SQL queries against an EF Core <see cref="DbContext"/> to retrieve
/// primary key values for entities of type <typeparamref name="TProjection"/>.
/// </summary>
/// <typeparam name="TProjection">
/// The EF Core entity or projection type being queried. Must be a reference type.
/// </typeparam>
/// <remarks>
/// This executor is intentionally thin and delegates all SQL execution to EF Core's
/// relational query pipeline. It is typically used by search orchestrators to obtain
/// primary key values for matched rows before rehydrating full entity instances via
/// LINQ-to-Objects.
/// </remarks>
[ExcludeFromCodeCoverage]
public sealed class SqlExecutor<TProjection> : ISqlExecutor<TProjection>
    where TProjection : class
{
    /// <summary>
    /// Executes a raw SQL query and returns the primary key values for the matching
    /// <typeparamref name="TProjection"/> entities.
    /// </summary>
    /// <param name="db">
    /// The EF Core <see cref="DbContext"/> used to execute the SQL query.
    /// </param>
    /// <param name="sql">
    /// The raw SQL query to execute. Must be valid SQL for the underlying database provider.
    /// </param>
    /// <param name="primaryKeyPropertyName">
    /// The name of the primary key property on <typeparamref name="TProjection"/> whose
    /// values should be extracted from the SQL result set.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A list of primary key values extracted from the SQL result set.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="db"/> or <paramref name="sql"/> is null.
    /// </exception>
    /// <remarks>
    /// This method relies on EF Core's <see cref="RelationalQueryableExtensions.FromSqlRaw{TEntity}(IQueryable{TEntity}, string, object[])"/>
    /// to execute the SQL query. Because <c>FromSqlRaw</c> is only supported by relational providers,
    /// this executor cannot be unit tested using EF Core's InMemory provider.
    /// </remarks>
    public async Task<List<object>> ExecuteIdsAsync(
        DbContext db,
        string sql,
        string primaryKeyPropertyName,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(db);
        ArgumentNullException.ThrowIfNull(sql);

        return await db.Set<TProjection>()
            .FromSqlRaw(sql)
            .Select(projection =>
                EF.Property<object>(projection, primaryKeyPropertyName))
            .ToListAsync(cancellationToken);
    }
}
