using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering.FilterExpressions;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering.FilterExpressions.Factories;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Search.Infrastructure.Core.Filtering.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchFilterExpressionFactoryTestDouble
{
    public static Mock<ISearchFilterExpressionFactory> Mock() =>
        new(MockBehavior.Strict);

    public static (Mock<ISearchFilterExpressionFactory> factory, Mock<ISearchFilterExpression> expression)
        MockFor(string filterKey, string expressionValue)
    {
        var exprMock = new Mock<ISearchFilterExpression>(MockBehavior.Strict);
        exprMock
            .Setup(searchFilterExpression =>
                searchFilterExpression.GetFilterExpression(It.IsAny<SearchFilterRequest>()))
            .Returns(expressionValue)
            .Verifiable();

        var factoryMock = Mock();

        factoryMock
            .Setup(searchFilterExpressionFactory =>
                searchFilterExpressionFactory.CreateFilter(filterKey))
            .Returns(exprMock.Object)
            .Verifiable();

        return (factoryMock, exprMock);
    }
}
