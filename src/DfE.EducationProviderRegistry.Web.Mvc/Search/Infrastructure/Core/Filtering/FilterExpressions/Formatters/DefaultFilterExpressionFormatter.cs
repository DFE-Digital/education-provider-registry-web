using System.Runtime.CompilerServices;
using System.Text;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering.FilterExpressions.Formatters;

/// <summary>
/// Provides a convenient mechanism for creating composite format strings for use when generating Azure AI OData
/// filter expressions in string format, which consist of fixed text intermixed with indexed placeholders that
/// correspond to the number of param objects defined in the filterCriteria list. The formatting operation yields
/// a result string that consists of the original fixed text intermixed with the string representation of the objects
/// in the list. Composite formatting is supported by methods such as String.Format, Console.WriteLine, and StringBuilder.AppendFormat.
/// The expression formatter leverages the <see cref="FormattableStringFactory"/>, please refer to this link for further information,
/// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.formattablestringfactory?view=net-8.0">Fomattable string factory details</a>.
/// </summary>
public sealed class DefaultFilterExpressionFormatter : IFilterExpressionFormatter
{
    /// <summary>
    /// The string value used to separate the filter placeholders in the generated formatted string.
    /// For example, we may choose to use a comma to separate values assigned to a search.in OData
    /// expression so the format string generated would look like the following,
    /// search.in('{0},{1},{2}') if we provisioned three values in the filterCriteria params.
    /// </summary>
    private string ExpressionParamsSeparator { get; set; } = string.Empty;

    /// <summary>
    /// Allows the settings of string value used to separate filter value
    /// placeholders in the formatted OData filter expression string intended.
    /// </summary>
    /// <param name="separator">
    /// The string value used to separate the filter placeholders.
    /// </param>
    public void SetExpressionParamsSeparator(string separator) { ExpressionParamsSeparator = separator; }

    /// <summary>
    /// Allows creation of a <see cref="FormattableString"/> used to create the intended OData filter expression
    /// based on the provisioned expression format string and object array containing zero or more objects to format.
    /// </summary>
    /// <param name="expressionFormat">
    /// The intended OData string used to establish the generated <see cref="FormattableString"/>.
    /// </param>
    /// <param name="filterCriteria">
    /// The collection of OData filter params used to determine the number of filter placeholders to establish.
    /// </param>
    /// <returns>
    /// A string which includes the formatted placeholder values assigned via
    /// the filterCriteria param collection, separated with the string value provisioned.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Exception thrown if the expressionFormat string is not provided.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Exception thrown if no filter criteria arguments are provided.
    /// </exception>
    public string CreateFormattedExpression(string expressionFormat, params object[] filterCriteria)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(expressionFormat);

        if (filterCriteria.Length == 0){
            throw new ArgumentException(
                "Filter argument cannot be null or empty", nameof(filterCriteria));
        }

        return FormattableStringFactory.Create(expressionFormat, filterCriteria).ToString();
    }

    /// <summary>
    /// Allows creation of the format string parameters based on the length of
    /// the filter criteria collection passed. For example, if we passed a collection
    /// with three values with a comma separator we could expect the following result, {0},{1},{2}.
    /// </summary>
    /// <param name="filterCriteria">
    /// The collection of OData filter placeholder values used to allow assignment of the
    /// parameter values provisioned to the OData filter formatted placeholder string.
    /// </param>
    /// <returns>
    /// The formatted placeholders, i.e. {0},{1},{2}.
    /// </returns>
    public string CreateFilterCriteriaPlaceholders(object[] filterCriteria)
    {
        StringBuilder filterCriteriaFormat = new();

        filterCriteriaFormat
            .AppendJoin(
                ExpressionParamsSeparator,
                Enumerable.Range(0, filterCriteria.Length).Select(placeholder => $"{{{placeholder}}}"));

        return filterCriteriaFormat.ToString();
    }
}
