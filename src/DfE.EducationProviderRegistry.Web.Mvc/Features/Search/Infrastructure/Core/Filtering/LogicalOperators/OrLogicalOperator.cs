using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Filtering.LogicalOperators.Extensions;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Filtering.LogicalOperators;

/// <summary>
/// Represents a logical OR operator used when composing filter expressions.
/// Logical operators allow multiple filter conditions to be combined into a
/// single expression by applying Boolean logic.
/// </summary>
public sealed class OrLogicalOperator : ILogicalOperator
{
    /// <summary>
    /// Constant string used to represent the logical OR operator.
    /// </summary>
    private const string LogicOperator = "OR";

    /// <summary>
    /// Returns the logical OR operator padded on both sides. Padding ensures
    /// the operator can be cleanly embedded between filter expression fragments.
    /// </summary>
    /// <returns>
    /// A padded string representing the logical OR operator.
    /// </returns>
    public string GetOperatorExpression() => LogicOperator.PadSides();
}
