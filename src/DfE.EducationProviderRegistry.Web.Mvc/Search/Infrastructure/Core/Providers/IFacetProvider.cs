using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers;

/// <summary>
///     Defines a contract for retrieving facet buckets for a given entity type.
///     A facet provider receives a set of entity identifiers and a facet name,
///     and returns aggregated facet buckets representing counts or groupings
///     relevant to the search domain (e.g., categories, brands, types).
/// </summary>
/// <typeparam name="TEntity">
///     The entity type for which facet information is being retrieved.
///     This type parameter ensures alignment with the search pipeline and
///     allows facet providers to operate on domain‑specific entity models.
/// </typeparam>
public interface IFacetProvider<TEntity>
    where TEntity : class
{
    /// <summary>
    ///     Retrieves facet buckets for the specified facet name, based on the
    ///     supplied list of entity identifiers. Implementations typically perform
    ///     grouping and counting operations to produce facet buckets that can be
    ///     displayed alongside search results.
    /// </summary>
    /// <param name="ids">
    ///     The identifiers of the entities that matched the search query.
    ///     Implementations should assume this list is non‑empty and should
    ///     only compute facet values for these entities.
    /// </param>
    /// <param name="facetName">
    ///     The name of the facet to compute (e.g., <c>"Category"</c>,
    ///     <c>"Brand"</c>). Implementations should validate that the facet
    ///     name corresponds to a supported facet.
    /// </param>
    /// <param name="cancellationToken">
    ///     A cancellation token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    ///     A read‑only list of <see cref="FacetResult"/> instances representing
    ///     the computed facet values and their associated counts.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="ids"/> or <paramref name="facetName"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="facetName"/> is empty or unsupported.
    /// </exception>
    /// <remarks>
    ///     Implementations should be thread‑safe and capable of being executed
    ///     concurrently, as facet providers may be invoked in parallel for
    ///     different facet names during a single search operation.
    /// </remarks>
    Task<IReadOnlyList<FacetResult>> GetFacetsAsync(
        IReadOnlyList<string> ids,
        string facetName,
        CancellationToken cancellationToken = default);
}