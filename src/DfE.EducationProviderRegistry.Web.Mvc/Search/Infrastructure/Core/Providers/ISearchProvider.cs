using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Providers.Projections;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers;

/// <summary>
///     Defines a contract for retrieving the identifiers of entities that match
///     a supplied search query. Implementations typically perform text‑based
///     similarity matching (e.g., PostgreSQL trigram search) and return the
///     ordered list of matching entity IDs.
/// </summary>
/// <typeparam name="TEntity">
///     The entity type associated with the underlying data source. This type
///     parameter ensures alignment with the search pipeline and allows ID
///     providers to be registered per‑entity.
/// </typeparam>
public interface ISearchProvider<TEntity>
{
    /// <summary>
    ///     Retrieves the identifiers of entities that match the supplied search
    ///     query. Implementations should apply the appropriate matching logic
    ///     (e.g., trigram similarity, full‑text search, keyword matching) and
    ///     return the resulting IDs in a deterministic order.
    /// </summary>
    /// <param name="query">
    ///     The search query containing the term, paging information, and any
    ///     additional parameters required by the ID provider.
    /// </param>
    /// <param name="cancellationToken">
    ///     A cancellation token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    ///     A read‑only list of matching entity identifiers. The returned list
    ///     should reflect the provider’s ranking or ordering strategy (e.g.,
    ///     descending similarity score).
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="query"/> is <c>null</c>.
    /// </exception>
    /// <remarks>
    ///     <para>
    ///         ID providers are responsible only for determining which entities
    ///         match the search term and in what order. They do not apply
    ///         filtering, projection, or facet logic — these responsibilities
    ///         belong to other components in the search pipeline.
    ///     </para>
    ///     <para>
    ///         Implementations should be thread‑safe and capable of being invoked
    ///         concurrently, as the search engine may execute multiple searches
    ///         in parallel.
    ///     </para>
    /// </remarks>
    Task<IReadOnlyList<SearchResultProjection>> GetMatchingIdsAsync(
        string searchTerm,
        int pageSize,
        int offset,
        IReadOnlyList<SearchFilterRequest> searchFilterRequests,
        CancellationToken cancellationToken = default);
}
