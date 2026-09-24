using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Datasets.ViewModels;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Download.Datasets.Controllers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class UseCaseReponseToViewModelMapperTestDouble
{
    public static Mock<IMapper<
        Dataset,
        DownloadedDatasetViewModel>> Mock() => new(MockBehavior.Strict);

    public static Mock<IMapper<
        Dataset,
        DownloadedDatasetViewModel>> MockFor(
        DownloadedDatasetViewModel viewModel,
        Dataset dataset)
    {
        Mock<IMapper<
            Dataset,
            DownloadedDatasetViewModel>> mock = Mock();

        mock
            .Setup(mapper =>
                mapper.Map(dataset))
            .Returns(viewModel)
            .Verifiable();

        return mock;
    }
}