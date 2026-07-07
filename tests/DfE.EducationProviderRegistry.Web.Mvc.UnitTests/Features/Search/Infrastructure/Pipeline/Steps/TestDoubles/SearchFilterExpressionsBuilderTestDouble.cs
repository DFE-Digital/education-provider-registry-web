using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Filtering;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Infrastructure.Pipeline.Steps.TestDoubles;

[ExcludeFromCodeCoverage]
internal class SearchFilterExpressionsBuilderTestDouble
{
    public static Mock<ISearchFilterExpressionsBuilder> Mock() =>
        new(MockBehavior.Strict);

    public static Mock<ISearchFilterExpressionsBuilder>
        MockFor(string filterExpression)
    {
        Mock<ISearchFilterExpressionsBuilder> searchFilterBuilderMock = Mock();

        searchFilterBuilderMock
            .Setup(filterBuilder =>
                filterBuilder.BuildSearchFilterExpressions(
                    It.IsAny<IReadOnlyList<SearchFilterRequest>>()))
                        .Returns(filterExpression);

        return searchFilterBuilderMock;
    }
}
