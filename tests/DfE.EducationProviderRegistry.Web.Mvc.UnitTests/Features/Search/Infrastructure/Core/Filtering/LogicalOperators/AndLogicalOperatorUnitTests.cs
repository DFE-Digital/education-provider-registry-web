using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Filtering.LogicalOperators;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Infrastructure.Core.Filtering.LogicalOperators;

public sealed class AndLogicalOperatorUnitTests
{
    [Fact]
    public void GetOperatorExpression_ReturnsPaddedAndOperator()
    {
        // arrange
        AndLogicalOperator logicalOperator = new();

        // act
        string result = logicalOperator.GetOperatorExpression();

        // assert
        Assert.Equal(" AND ", result);
    }

    [Fact]
    public void GetOperatorExpression_AlwaysReturnsUppercaseAnd()
    {
        // arrange
        AndLogicalOperator logicalOperator = new();

        // act
        string result = logicalOperator.GetOperatorExpression();

        // assert
        Assert.Contains("AND", result);
    }

    [Fact]
    public void GetOperatorExpression_PaddingIsAppliedOnBothSides()
    {
        // arrange
        AndLogicalOperator logicalOperator = new();

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
        AndLogicalOperator logicalOperator = new();

        // act
        string result = logicalOperator.GetOperatorExpression();

        // assert
        Assert.False(string.IsNullOrWhiteSpace(result));
    }
}
