namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Filtering.FilterExpressions;

/// <summary>
/// Defines an abstraction for producing a filter expression string from a
/// <see cref="SearchFilterRequest"/>. Implementations may generate expressions
/// in different syntaxes (e.g., SQL, OData, or provider‑specific formats)
/// depending on the underlying search technology.
/// </summary>
public interface ISearchFilterExpression
{
    /// <summary>
    /// Produces a filter expression string based on the details supplied in the
    /// <see cref="SearchFilterRequest"/>. The concrete implementation determines
    /// the syntax and semantics of the resulting expression.
    /// </summary>
    /// <param name="searchFilterRequest">
    /// Encapsulates the filter key and associated values used to construct the
    /// filter expression.
    /// </param>
    /// <returns>
    /// A filter expression string appropriate for the target search provider.
    /// </returns>
    string GetFilterExpression(SearchFilterRequest searchFilterRequest);
}