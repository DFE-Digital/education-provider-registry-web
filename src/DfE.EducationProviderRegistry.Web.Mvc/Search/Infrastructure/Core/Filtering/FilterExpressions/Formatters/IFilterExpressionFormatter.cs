namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering.FilterExpressions.Formatters;

/// <summary>
/// Abstraction used to define the behaviour prescribed to the mechanism
/// used to generate the required Azure AI OData filter expressions.
/// </summary>
public interface IFilterExpressionFormatter
{
    /// <summary>
    /// Allows the settings of string value used to separate filter value
    /// placeholders in the formatted OData filter expression string intended.
    /// </summary>
    /// <param name="separator">
    /// The string value used to separate the filter placeholders.
    /// </param>
    void SetExpressionParamsSeparator(string separator);

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
    string CreateFilterCriteriaPlaceholders(object[] filterCriteria);

    /// <summary>
    /// Allows creation of a string which represents the intended OData filter expression
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
    string CreateFormattedExpression(string expressionFormat, params object[] filterCriteria);
}