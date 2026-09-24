using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Response;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Download.Datasets.Controllers.TestDoubles;

[ExcludeFromCodeCoverage]
internal class DownloadDatasetsUseCaseTestDouble
{
    public static Mock<IUseCase<
        DownloadDatasetsRequest,
        UseCaseResponse<DownloadDatasetsResponse>>> Mock() => new(MockBehavior.Strict);

    public static Mock<IUseCase<
        DownloadDatasetsRequest,
        UseCaseResponse<DownloadDatasetsResponse>>> MockFor(
        UseCaseResponse<DownloadDatasetsResponse> response)
    {
        Mock<IUseCase<
            DownloadDatasetsRequest,
            UseCaseResponse<DownloadDatasetsResponse>>> mock = Mock();

        mock
            .Setup(useCase =>
                useCase.HandleRequestAsync(It.IsAny<DownloadDatasetsRequest>()))
            .ReturnsAsync(response)
            .Verifiable();

        return mock;
    }
}