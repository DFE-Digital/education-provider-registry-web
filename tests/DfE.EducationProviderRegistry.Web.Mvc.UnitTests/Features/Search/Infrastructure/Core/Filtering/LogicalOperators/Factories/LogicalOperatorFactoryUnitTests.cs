using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Filtering.LogicalOperators;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Filtering.LogicalOperators.Factories;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Infrastructure.Core.Filtering.LogicalOperators.Factories;

public sealed class LogicalOperatorFactoryUnitTests
{
    private sealed class TestLogicalOperatorA : ILogicalOperator
    {
        public string GetOperatorExpression() => "AND";
    }

    private sealed class TestLogicalOperatorB : ILogicalOperator
    {
        public string GetOperatorExpression() => "OR";
    }

    private LogicalOperatorFactory CreateFactory()
    {
        Dictionary<string, Func<ILogicalOperator>> dictionary =
            new()
            {
                { nameof(TestLogicalOperatorA), () => new TestLogicalOperatorA() },
                { nameof(TestLogicalOperatorB), () => new TestLogicalOperatorB() }
            };

        return new LogicalOperatorFactory(dictionary);
    }

    [Fact]
    public void CreateLogicalOperator_ValidName_ReturnsCorrectInstance_A()
    {

        // arrange
        LogicalOperatorFactory factory = CreateFactory();

        // act
        ILogicalOperator result =
            factory.CreateLogicalOperator(nameof(TestLogicalOperatorA));

        // assert
        Assert.IsType<TestLogicalOperatorA>(result);
        Assert.Equal("AND", result.GetOperatorExpression());
    }

    [Fact]
    public void CreateLogicalOperator_ValidName_ReturnsCorrectInstance_B()
    {
        // arrange
        LogicalOperatorFactory factory = CreateFactory();

        // act
        ILogicalOperator result =
            factory.CreateLogicalOperator(nameof(TestLogicalOperatorB));

        // assert
        Assert.IsType<TestLogicalOperatorB>(result);
        Assert.Equal("OR", result.GetOperatorExpression());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateLogicalOperator_InvalidName_ThrowsArgumentException(string name)
    {
        // arrange
        LogicalOperatorFactory factory = CreateFactory();

        // assert
        Assert.Throws<ArgumentException>(() =>
            factory.CreateLogicalOperator(name));
    }

    [Fact]
    public void CreateLogicalOperator_UnknownName_ThrowsArgumentOutOfRangeException()
    {
        // arrange
        LogicalOperatorFactory factory = CreateFactory();

        // assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            factory.CreateLogicalOperator("DoesNotExist"));
    }

    [Fact]
    public void CreateLogicalOperator_DelegateIsInvoked()
    {
        // arrange
        Boolean invoked = false;

        Dictionary<string, Func<ILogicalOperator>> dictionary =
            new()
            {
                {
                    "TestOperator",
                    () =>
                    {
                        invoked = true;
                        return new TestLogicalOperatorA();
                    }
                }
            };

        LogicalOperatorFactory factory = new(dictionary);

        // act
        ILogicalOperator result = factory.CreateLogicalOperator("TestOperator");

        // assert
        Assert.True(invoked);
        Assert.IsType<TestLogicalOperatorA>(result);
        Assert.Equal("AND", result.GetOperatorExpression());
    }
}
