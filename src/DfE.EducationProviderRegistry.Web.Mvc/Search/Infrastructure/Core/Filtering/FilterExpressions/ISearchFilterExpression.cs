namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering.FilterExpressions;

/// <summary>
/// Provides an abstraction over which to derive Azure AI OData filter expressions. For more information on OData filter
/// syntax in Azure AI search please see, <a href="https://learn.microsoft.com/en-us/azure/search/search-query-odata-filter">OData search query filters</a>.
/// </summary>
public interface ISearchFilterExpression
{
    /// <summary>
    /// Allows a filter expression to be derived in string format based on the requirements specified in the <see cref="SearchFilterRequest"/>.
    /// </summary>
    /// <param name="searchFilterRequest">
    /// The <see cref="SearchFilterRequest"/> object used to encapsulate filter request details which includes the filter key and associated values.
    /// </param>
    /// <returns>
    /// A configured OData Azure AI filter expression in string format.
    /// </returns>
    string GetFilterExpression(SearchFilterRequest searchFilterRequest);
}