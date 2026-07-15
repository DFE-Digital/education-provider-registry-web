using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Controllers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchUseCaseTestDouble
{
    public static Mock<IUseCase<
        SearchRequest, UseCaseResponse<SearchResponse>>> Mock() => new(MockBehavior.Strict);

    public static Mock<IUseCase<SearchRequest, UseCaseResponse<SearchResponse>>> MockFor(
        UseCaseResponse<SearchResponse> response)
    {
        Mock<IUseCase<
            SearchRequest, UseCaseResponse<SearchResponse>>> mock = Mock();

        mock
            .Setup(useCase =>
                useCase.HandleRequestAsync(
                    It.IsAny<SearchRequest>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(response)
            .Verifiable();

        return mock;
    }
}
