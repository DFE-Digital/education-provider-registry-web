using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering.LogicalOperators;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Search.Infrastructure.Core.Filtering.LogicalOperators;

public sealed class OrLogicalOperatorUnitTests
{
    [Fact]
    public void GetOperatorExpression_ReturnsPaddedOrOperator()
    {
        // arrange
        OrLogicalOperator logicalOperator = new();

        // act
        string result = logicalOperator.GetOperatorExpression();

        // assert
        Assert.Equal(" OR ", result);
    }

    [Fact]
    public void GetOperatorExpression_AlwaysReturnsUppercaseOr()
    {
        // arrange
        OrLogicalOperator logicalOperator = new();

        // act
        string result = logicalOperator.GetOperatorExpression();

        // assert
        Assert.Contains("OR", result);
    }

    [Fact]
    public void GetOperatorExpression_PaddingIsAppliedOnBothSides()
    {
        // arrange
        OrLogicalOperator logicalOperator = new();

        // act
        string result = logicalOperator.GetOperatorExpression();

        // assert
        Assert.StartsWith(" ", result);
        Assert.EndsWith(" ", result);
    }

    [Fact]
    public void GetOperatorExpression_ReturnsNonEmptyString()
    {
        // arrange
        OrLogicalOperator logicalOperator = new OrLogicalOperator();

        // act
        string result = logicalOperator.GetOperatorExpression();

        // assert
        Assert.False(string.IsNullOrWhiteSpace(result));
    }
}
