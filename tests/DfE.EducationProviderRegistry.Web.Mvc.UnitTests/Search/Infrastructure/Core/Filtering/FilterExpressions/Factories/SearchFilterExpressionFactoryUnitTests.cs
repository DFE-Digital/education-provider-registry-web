using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering.FilterExpressions;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering.FilterExpressions.Factories;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Search.Infrastructure.Core.Filtering.FilterExpressions.Factories;

public sealed class SearchFilterExpressionFactoryUnitTests
{
    private sealed class TestFilterExpressionA : ISearchFilterExpression
    {
        public string GetFilterExpression(SearchFilterRequest searchFilterRequest) => "A";
    }

    private sealed class TestFilterExpressionB : ISearchFilterExpression
    {
        public string GetFilterExpression(SearchFilterRequest searchFilterRequest) => "B";
    }

    private SearchFilterExpressionFactory CreateFactory()
    {
        Dictionary<string, Func<ISearchFilterExpression>> dictionary =
            new()
            {
                { nameof(TestFilterExpressionA), () => new TestFilterExpressionA() },
                { nameof(TestFilterExpressionB), () => new TestFilterExpressionB() }
            };

        return new SearchFilterExpressionFactory(dictionary);
    }

    [Fact]
    public void CreateFilter_ByGenericType_ReturnsCorrectInstance()
    {
        // arrange
        SearchFilterExpressionFactory factory = CreateFactory();

        // act
        ISearchFilterExpression result =
            factory.CreateFilter<TestFilterExpressionA>();

        // assert
        Assert.IsType<TestFilterExpressionA>(result);
    }

    [Fact]
    public void CreateFilter_ByType_ReturnsCorrectInstance()
    {
        // arrange
        SearchFilterExpressionFactory factory = CreateFactory();

        // act
        ISearchFilterExpression result =
            factory.CreateFilter<TestFilterExpressionB>();

        // assert
        Assert.IsType<TestFilterExpressionB>(result);
    }

    [Fact]
    public void CreateFilter_ByName_ReturnsCorrectInstance()
    {
        // arrange
        SearchFilterExpressionFactory factory = CreateFactory();

        // act
        ISearchFilterExpression result =
            factory.CreateFilter(nameof(TestFilterExpressionA));

        // assert
        Assert.IsType<TestFilterExpressionA>(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateFilter_InvalidName_ThrowsArgumentException(string filterName)
    {
        // arrange
        SearchFilterExpressionFactory factory = CreateFactory();

        // assert
        Assert.Throws<ArgumentException>(() =>
            factory.CreateFilter(filterName));
    }

    [Fact]
    public void CreateFilter_UnknownName_ThrowsArgumentOutOfRangeException()
    {
        // arrange
        SearchFilterExpressionFactory factory = CreateFactory();

        // assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            factory.CreateFilter("DoesNotExist"));
    }

    [Fact]
    public void CreateFilter_DelegateIsInvoked()
    {
        // arrange
        Boolean invoked = false;

        Dictionary<string, Func<ISearchFilterExpression>> dictionary =
            new()
            {
                {
                    "Test",
                    () =>
                    {
                        invoked = true;
                        return new TestFilterExpressionA();
                    }
                }
            };

        SearchFilterExpressionFactory factory = new(dictionary);

        // act
        ISearchFilterExpression result = factory.CreateFilter("Test");

        // assert
        Assert.True(invoked);
        Assert.IsType<TestFilterExpressionA>(result);
    }
}
