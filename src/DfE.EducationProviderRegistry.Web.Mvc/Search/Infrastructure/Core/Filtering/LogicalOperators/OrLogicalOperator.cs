using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering.LogicalOperators.Extensions;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering.LogicalOperators;

/// <summary>
/// Creates an OData logical operator filter expression which are Boolean expressions that evaluate to true or false.
/// This allows us to compose more complex filter expressions by writing a series of simpler filters and composing them
/// using the logical operators from Boolean algebra. The <b>or</b> binary operator evaluates to true if either one of its
/// left or right sub-expressions evaluates to true. For further information please visit the following link,
/// <a href="https://learn.microsoft.com/en-us/azure/search/search-query-odata-logical-operators"> OData logical operators</a>.
/// </summary>
public sealed class OrLogicalOperator : ILogicalOperator
{
    /// <summary>
    /// Constant string used to compose the <b>or</b> logical operator.
    /// </summary>
    const string LogicOperator = "OR";

    /// <summary>
    /// Returns the configured <b>or</b> logical operator by applying default padding to
    /// both sides of the operator. This allows the operator to be easily nested between filter expressions.
    /// </summary>
    /// <returns>
    /// A string representing the configured <b>or</b> logical operator.
    /// </returns>
    public string GetOperatorExpression() => LogicOperator.PadSides();
}
