namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Filtering;

/// <summary>
/// Provides an abstraction over which to derive Azure AI OData filter expression builder. The abstraction defines
/// the collection of <see cref="SearchFilterRequest"/> types which are used to reconcile to the underlying OData
/// filter expressions used to construct the required OData filter(s) composition.
/// </summary>
public interface ISearchFilterExpressionsBuilder
{
    /// <summary>
    /// Allows the construction of an OData filter expression(s) composition in string format.
    /// </summary>
    /// <param name="searchFilterRequests">
    /// The collection of <see cref="SearchFilterRequest"/> types which are used to reconcile
    /// to the underlying OData filter expressions.
    /// </param>
    /// <returns>
    /// A string which represents the formatted OData filter expression(s) composition.
    /// </returns>
    string BuildSearchFilterExpressions(IEnumerable<SearchFilterRequest> searchFilterRequests);
}