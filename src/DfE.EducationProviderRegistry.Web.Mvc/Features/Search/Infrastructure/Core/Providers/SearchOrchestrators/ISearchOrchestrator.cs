using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Providers.SearchOrchestrators.Context;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Providers.SearchOrchestrators;

/// <summary>
/// Defines the contract for executing a search orchestration pipeline that applies
/// database-backed similarity matching, filtering, and paging logic, returning a
/// projected result set of <typeparamref name="TProjection"/> instances.
/// </summary>
/// <typeparam name="TProjection">
/// The entity or projection type being queried. Must be a reference type.
/// </typeparam>
/// <remarks>
/// Implementations typically combine raw SQL execution (e.g., trigram similarity queries)
/// with LINQ-based rehydration of full entity or projection instances. This abstraction
/// allows search providers to remain database-agnostic while enabling specialised search
/// behaviour such as PostgreSQL <c>pg_trgm</c> similarity ranking.
/// </remarks>
public interface ISearchOrchestrator<TProjection>
    where TProjection : class
{
    /// <summary>
    /// Executes a search operation using the provided EF Core <see cref="DbContext"/>,
    /// base LINQ query, and search context, returning a filtered and ordered result set.
    /// </summary>
    /// <param name="db">
    /// The EF Core <see cref="DbContext"/> used to execute any required SQL queries.
    /// </param>
    /// <param name="baseQuery">
    /// The base LINQ <see cref="IQueryable{TProjection}"/> used to rehydrate full entity
    /// or projection instances after primary key values have been retrieved via SQL.
    /// </param>
    /// <param name="context">
    /// The search context containing the search term, paging information, filters,
    /// and the column to apply similarity matching against.
    /// </param>
    /// <param name="searchFilters">
    /// Optional additional SQL filter clauses appended directly to the underlying
    /// search query. Must be valid SQL. Defaults to an empty string.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A read-only list of <typeparamref name="TProjection"/> instances matching the
    /// search criteria defined in <paramref name="context"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="db"/> or <paramref name="context"/> is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the search column specified in <paramref name="context"/> does not
    /// exist on the underlying EF Core entity type.
    /// </exception>
    Task<IReadOnlyList<TProjection>> ExecuteAsync(
        DbContext db,
        IQueryable<TProjection> baseQuery,
        SearchOrchestratorContext context,
        string searchFilters = "",
        CancellationToken cancellationToken = default);
}