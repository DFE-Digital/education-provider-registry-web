using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering.FilterExpressions;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering.FilterExpressions.Factories;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering.LogicalOperators;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering.Options;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;
using System.Text;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering;

/// <summary>
/// Composes and builds filter expression strings based on incoming request keys and values.
/// These keys are mapped to configured filter expression types, which determine how values
/// are formatted and combined into a final filter expression.
/// 
/// For example, given:
/// <code>
/// List&lt;SearchFilterRequest&gt; searchFilterRequests =
///     SearchFilterRequestBuilder.Create().BuildSearchFilterRequestsWith(
///         ("OFSTEDRATINGCODE", new List&lt;object&gt; { "2", "5", "9", "12" }),
///         ("RELIGIOUSCHARACTERCODE", new List&lt;object&gt; { "00", "02" }))
///            .BuildSearchFilterRequests();
/// </code>
/// 
/// And configuration:
/// <code>
/// "FilterKeyToFilterExpressionMapOptions": {
///     "FilterChainingLogicalOperator": "AndLogicalOperator",
///     "SearchFilterToExpressionMap": {
///         "RELIGIOUSCHARACTERCODE": {
///             "FilterExpressionKey": "SearchInFilterExpression",
///             "FilterExpressionValuesDelimiter": ","
///         },
///         "OFSTEDRATINGCODE": {
///             "FilterExpressionKey": "SearchInFilterExpression",
///             "FilterExpressionValuesDelimiter": ","
///         }
///     }
/// }
/// </code>
/// 
/// The resulting filter expression string would be:
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
    /// Constructs a <see cref="SearchFilterExpressionsBuilder"/> using:
    /// - a <see cref="ISearchFilterExpressionFactory"/> to resolve filter expression types,
    /// - a <see cref="ILogicalOperatorFactory"/> to resolve logical operators,
    /// - and <see cref="FilterKeyToFilterExpressionMapOptions"/> to map incoming filter keys
    ///   to specific filter expression configurations.
    /// </summary>
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
    /// Builds a composed filter expression string based on the incoming filter requests.
    /// Each request is mapped to a configured filter expression type and combined using
    /// the configured logical operator.
    /// </summary>
    public string BuildSearchFilterExpressions(IEnumerable<SearchFilterRequest> searchFilterRequests)
    {
        IEnumerable<string> searchFilters = GetValidSearchFilterExpression(searchFilterRequests);
        ILogicalOperator logicalOperator = GetFilterChainingLogicalOperator();

        _aggregatedSearchFilterExpression.AppendJoin(logicalOperator.GetOperatorExpression(), searchFilters);

        return _aggregatedSearchFilterExpression.ToString();
    }

    /// <summary>
    /// Resolves filter expression types based on incoming filter keys. Only keys present
    /// in the configured filter expression map are processed.
    /// </summary>
    private ReadOnlyCollection<string> GetValidSearchFilterExpression(IEnumerable<SearchFilterRequest> searchFilterRequests)
    {
        List<string> searchFilters = [];

        foreach (SearchFilterRequest searchFilterRequest in searchFilterRequests
            .Where(searchFilterRequest =>
                _filterKeyToFilterExpressionMapOptions
                    .SearchFilterToExpressionMap.ContainsKey(searchFilterRequest.FilterKey)))
        {
            FilterExpressionOptions filterExpressionOptions =
                _filterKeyToFilterExpressionMapOptions.SearchFilterToExpressionMap[searchFilterRequest.FilterKey];

            if (filterExpressionOptions.HasValuesDelimiter)
            {
                searchFilterRequest.SetFilterValuesDelimiter(filterExpressionOptions.FilterExpressionValuesDelimiter);
            }

            ISearchFilterExpression searchFilterExpression =
                _searchFilterExpressionFactory.CreateFilter(filterExpressionOptions.FilterExpressionKey);

            searchFilters.Add(searchFilterExpression.GetFilterExpression(searchFilterRequest));
        }

        return searchFilters.AsReadOnly();
    }

    /// <summary>
    /// Resolves the logical operator used to chain multiple filter expressions together.
    /// </summary>
    private ILogicalOperator GetFilterChainingLogicalOperator()
    {
        string filterChainingLogicalOperatorKey =
            !string.IsNullOrWhiteSpace(_filterKeyToFilterExpressionMapOptions.FilterChainingLogicalOperator)
                ? _filterKeyToFilterExpressionMapOptions.FilterChainingLogicalOperator
                : throw new ArgumentException("Unable to assign a null or empty logical operator to the search expression chain.");

        return _logicalOperatorFactory.CreateLogicalOperator(filterChainingLogicalOperatorKey);
    }
}
