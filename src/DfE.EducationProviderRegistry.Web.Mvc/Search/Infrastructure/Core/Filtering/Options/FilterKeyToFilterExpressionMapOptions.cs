using System.ComponentModel.DataAnnotations;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering.Options;

/// <summary>
/// Configuration options for establishing a map to align the incoming filter request
/// key(s) with an available filter expression and logical operator. For example, we could
/// create a configuration section as follows,
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
/// If we provide an appropriate set of filter values aligned with the keys 'RELIGIOUSCHARACTERCODE'
/// and 'OFSTEDRATINGCODE' then the following OData filter expression string should be generated,
/// <code>
///     "search.in(OFSTEDRATINGCODE, '2,5,9,12') and search.in(RELIGIOUSCHARACTERCODE, '00,02')"
/// </code>
/// </summary>
public sealed class FilterKeyToFilterExpressionMapOptions
{
    /// <summary>
    /// The default logical operator is specified by key as either an <b>AndLogicalOperator</b>
    /// or an <b>OrLogicalOperator</b>. The logical operator is then used to chain together all
    /// search filter expressions specified.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string? FilterChainingLogicalOperator { get; set;  }

    /// <summary>
    /// The dictionary used to reconcile the incoming request key with the actual search filter expression to apply.
    /// </summary>
    [Required]
    [MinLength(1)]
    public IDictionary<string, FilterExpressionOptions> SearchFilterToExpressionMap { get; set; } = new Dictionary<string, FilterExpressionOptions>();
}

/// <summary>
/// Configuration options for establishing the specific filter expression options that are defined
/// under the <see cref="FilterKeyToFilterExpressionMapOptions"/>. This options section allows
/// for the specification of the required filter expression key (i.e. the named filter expression
/// such as 'SearchInFilterExpression' or 'LessThanOrEqualToExpression' for example, and the
/// accompanying value delimiter if required (i.e. for search.in we generally specify a comma
/// delimiter so an expression which encapsulates values with whitespace can be accommodated).
/// </summary>
public sealed class FilterExpressionOptions
{
    /// <summary>
    /// The key to the actual filter expression instance to derive. This (by convention) is defined
    /// by default in the DI container and uses the class name as the key to resolve the required
    /// filter expression type, e.g. 'SearchInFilterExpression'.
    /// </summary>
    public string FilterExpressionKey { get; set; } = string.Empty;

    /// <summary>
    /// The delimiter can be applied (optionally) for those expression types that require a
    /// delimiter to be specified between provisioned values, such as the search.in expression.
    /// This allows the underlying Azure AI search mechanism to correctly delimiter values
    /// even if whitespace is present.
    /// </summary>
    public string FilterExpressionValuesDelimiter { get; set; } = string.Empty;

    /// <summary>
    /// Check to determine whether a filter value delimiter has been specified.
    /// </summary>
    public bool HasValuesDelimiter => !string.IsNullOrWhiteSpace(FilterExpressionValuesDelimiter);
}
