namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering;

/// <summary>
/// Type used to encapsulate search filter requests by defining the filter key, and
/// associated values used to reconcile to an available OData filter expression
/// <see cref="ISearchFilterExpression"/> type.
/// </summary>
public sealed class SearchFilterRequest
{
    /// <summary>
    /// The key used to reconcile to the configured <see cref="ISearchFilterExpression"/> type
    /// derived using the configured <see cref="ISearchFilterExpressionFactory"/> instance.
    /// </summary>
    public string FilterKey { get; }

    /// <summary>
    /// The values to be applied to the reconciled OData filter expression type.
    /// </summary>
    public object[] FilterValues { get; }

    /// <summary>
    /// The string to be used by the specified filter expression to delimit the filter values provisioned.
    /// </summary>
    public string FilterValuesDelimiter { get; private set; } = string.Empty;

    /// <summary>
    /// Constructor ensures immutability for filter key and filter values provisioned.
    /// </summary>
    /// <param name="filterKey">
    /// The filter key string used to try and reconcile to the actual OData filter expression type.
    /// </param>
    /// <param name="filterValues">
    /// The collection of values to apply to the OData filter expression type.
    /// </param>
    /// <exception cref="ArgumentException">
    /// The exception thrown if no filter key is provisioned.
    /// </exception>
    /// /// <exception cref="ArgumentNullException">
    /// The exception thrown if no filter values are provisioned.
    /// </exception>
    public SearchFilterRequest(string filterKey, IEnumerable<object> filterValues)
    {
        ArgumentException.ThrowIfNullOrEmpty(filterKey);
        ArgumentNullException.ThrowIfNull(filterValues);

        if (!filterValues.Any())
        {
            throw new ArgumentException(
                "Filter values are required to build search filter arguments", nameof(filterValues));
        }

        FilterKey = filterKey;
        FilterValues = filterValues.ToArray();
    }

    /// <summary>
    /// Allows a filter value delimiter to be specified, if required by the underlying expression type.
    /// </summary>
    /// <param name="filterValuesDelimiter">
    /// The string used to delimit filter values.
    /// </param>
    public void SetFilterValuesDelimiter(string filterValuesDelimiter)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(filterValuesDelimiter);

        FilterValuesDelimiter = filterValuesDelimiter;
    }
}