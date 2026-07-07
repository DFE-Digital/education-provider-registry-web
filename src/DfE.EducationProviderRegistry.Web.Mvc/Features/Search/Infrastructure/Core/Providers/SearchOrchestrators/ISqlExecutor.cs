using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Providers.SearchOrchestrators;

/// <summary>
/// Defines an abstraction for executing raw SQL queries that return primary key values
/// for entities of type <typeparamref name="TProjection"/>.
/// </summary>
/// <typeparam name="TProjection">
/// The EF Core entity or projection type being queried. Must be a reference type.
/// </typeparam>
/// <remarks>
/// This interface allows search orchestrators to retrieve primary key values using
/// database-specific SQL (e.g., PostgreSQL trigram queries) without directly invoking
/// EF Core's relational <c>FromSqlRaw</c> pipeline. Implementations typically delegate
/// SQL execution to EF Core and return materialised primary key values.
/// </remarks>
public interface ISqlExecutor<TProjection>
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
    Task<List<object>> ExecuteIdsAsync(
        DbContext db,
        string sql,
        string primaryKeyPropertyName,
        CancellationToken cancellationToken = default);
}
