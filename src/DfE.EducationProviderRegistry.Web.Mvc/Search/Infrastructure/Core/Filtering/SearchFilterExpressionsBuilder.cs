using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering.FilterExpressions;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering.FilterExpressions.Factories;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering.LogicalOperators;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering.Options;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;
using System.Text;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering;

/// <summary>
/// Used to compose and build Azure AI OData filter expression(s) based on the incoming request keys and values,
/// which are used to reconcile to the underlying search filter expressions and apply the associated values.
/// For example, if we create a request as follows,
/// <code>
/// List&lt;SearchFilterRequest&gt;searchFilterRequests =
///     SearchFilterRequestBuilder.Create().BuildSearchFilterRequestsWith(
///         ("OFSTEDRATINGCODE", new List&lt;object&gt; { "2", "5", "9", "12" }),
///         ("RELIGIOUSCHARACTERCODE", new List&lt;object&gt; { "00", "02" }))
///            .BuildSearchFilterRequests();
/// </code>
/// and we have the following configuration section,
/// <code>
/// "FilterKeyToFilterExpressionMapOptions": {
///     "FilterChainingLogicalOperator": "AndLogicalOperator",
///     "SearchFilterToExpressionMap": {
///         "RELIGIOUSCHARACTERCODE": {
///             "FilterExpressionKey": "SearchInFilterExpression",
///             "FilterExpressionValuesDelimiter", ","
///         },
///         "OFSTEDRATINGCODE": {
///             "FilterExpressionKey": "SearchInFilterExpression",
///             "FilterExpressionValuesDelimiter", ","
///         }
///     }
/// }
/// </code>
/// then the following OData filter expression string should be generated,
/// <code>
///     "search.in(OFSTEDRATINGCODE, '2,5,9,12') and search.in(RELIGIOUSCHARACTERCODE, '00,02')"
/// </code>
/// </summary>
public sealed class SearchFilterExpressionsBuilder : ISearchFilterExpressionsBuilder
{
    private readonly ISearchFilterExpressionFactory _searchFilterExpressionFactory;
    private readonly ILogicalOperatorFactory _logicalOperatorFactory;
    private readonly StringBuilder _aggregatedSearchFilterExpression = new();
    private readonly FilterKeyToFilterExpressionMapOptions _filterKeyToFilterExpressionMapOptions;

    /// <summary>
    /// The <see cref="SearchFilterExpressionsBuilder"/> uses a <see cref="ISearchFilterExpressionFactory"/>
    /// to derive all available search filter expressions available. Likewise, the <see cref="ILogicalOperatorFactory"/>
    /// is used to derive all available logical operators available. The <see cref="FilterKeyToFilterExpressionMapOptions"/>
    /// is used to derive the map of the incoming key to actual OData filter expression type to apply through-out the
    /// composition and build process.
    /// </summary>
    /// <param name="searchFilterExpressionFactory">
    /// Provides a factory implementation of <see cref="ISearchFilterExpressionFactory"/> over which to derive Azure AI OData filter
    /// expressions. This factory leverages dependency injection which necessitates
    /// setup of a dictionary of <see cref="ISearchFilterExpression"/> delegates.
    /// responsible for handling the creation of concrete <see cref="ISearchFilterExpression"/>
    /// instances.
    /// </param>
    /// <param name="logicalOperatorFactory">
    /// Provides a factory implementation of <see cref="ILogicalOperatorFactory"/> over which to derive Azure AI OData logical
    /// operator expressions. This factory leverages dependency injection which necessitates
    /// setup of a dictionary of <see cref="ILogicalOperator"/> delegates.
    /// responsible for handling the creation of concrete <see cref="ILogicalOperator"/>
    /// instances.
    /// </param>
    /// <param name="filterKeyToFilterExpressionMapOptions">
    /// Provides configuration options for establishing a map to align the incoming filter request
    /// key(s) with an available filter expression and logical operator.
    /// </param>
    public SearchFilterExpressionsBuilder(
        ISearchFilterExpressionFactory searchFilterExpressionFactory,
        ILogicalOperatorFactory logicalOperatorFactory,
        IOptions<FilterKeyToFilterExpressionMapOptions> filterKeyToFilterExpressionMapOptions)
    {
        _searchFilterExpressionFactory = searchFilterExpressionFactory;
        _logicalOperatorFactory = logicalOperatorFactory;
        ArgumentNullException.ThrowIfNull(filterKeyToFilterExpressionMapOptions);
        _filterKeyToFilterExpressionMapOptions = filterKeyToFilterExpressionMapOptions.Value;
    }

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
    public string BuildSearchFilterExpressions(IEnumerable<SearchFilterRequest> searchFilterRequests)
    {
        IEnumerable<string> searchFilters = GetValidSearchFilterExpression(searchFilterRequests);
        ILogicalOperator logicalOperator = GetFilterChainingLogicalOperator();

        _aggregatedSearchFilterExpression.AppendJoin(logicalOperator.GetOperatorExpression(), searchFilters);

        return _aggregatedSearchFilterExpression.ToString();
    }

    /// <summary>
    /// Allows for the extraction of OData filter expression types based on the incoming filter key which
    /// is reconciled with the configuration filter expression map to derive the required <see cref="ISearchFilterExpression"/>
    /// type from the factory provisioned.
    /// </summary>
    /// <param name="searchFilterRequests">
    /// The collection of incoming <see cref="SearchFilterRequest"/> objects which carry the search filter and
    /// values which are to be reconciled to the underlying OData search filter expression.
    /// </param>
    /// <returns>
    /// A collection of configured OData search filter expressions and values in string format.
    /// </returns>
    private ReadOnlyCollection<string> GetValidSearchFilterExpression(IEnumerable<SearchFilterRequest> searchFilterRequests)
    {
        List<string> searchFilters = [];

        // Only derive search expressions that are recognised through the filter key to expression map.
        foreach (SearchFilterRequest searchFilterRequest in searchFilterRequests
            .Where(searchFilterRequest =>
                _filterKeyToFilterExpressionMapOptions
                    .SearchFilterToExpressionMap.ContainsKey(searchFilterRequest.FilterKey)))
        {
            FilterExpressionOptions filterExpressionOptions =
                _filterKeyToFilterExpressionMapOptions.SearchFilterToExpressionMap[searchFilterRequest.FilterKey];

            // Assign the configured value delimiter if one is present.
            if (filterExpressionOptions.HasValuesDelimiter){
                searchFilterRequest.SetFilterValuesDelimiter(filterExpressionOptions.FilterExpressionValuesDelimiter);
            }

            ISearchFilterExpression searchFilterExpression =
                _searchFilterExpressionFactory.CreateFilter(filterExpressionOptions.FilterExpressionKey);

            searchFilters.Add(searchFilterExpression.GetFilterExpression(searchFilterRequest));
        }

        return searchFilters.AsReadOnly();
    }

    /// <summary>
    /// Allows for the extraction of an <see cref="ILogicalOperator"/> based on the
    /// key value specified in the configuration filter expression map.
    /// </summary>
    /// <returns>
    /// A configured <see cref="ILogicalOperator"/> instance of the type specified.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Exception thrown if the requested <see cref="ILogicalOperator"/> cannot be derived via the factory provisioned.
    /// </exception>
    private ILogicalOperator GetFilterChainingLogicalOperator()
    {
        string filterChainingLogicalOperatorKey =
            !string.IsNullOrWhiteSpace(_filterKeyToFilterExpressionMapOptions.FilterChainingLogicalOperator) ?
            _filterKeyToFilterExpressionMapOptions.FilterChainingLogicalOperator :
                throw new ArgumentException("Unable to assign a null or empty logical operator to the search expression chain.");

        return _logicalOperatorFactory.CreateLogicalOperator(filterChainingLogicalOperatorKey);
    }
}