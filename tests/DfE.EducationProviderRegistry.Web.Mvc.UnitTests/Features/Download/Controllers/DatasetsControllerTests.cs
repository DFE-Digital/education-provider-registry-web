using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Models;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Datasets.Controllers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Datasets.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Download.Controllers.TestDoubles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Text.Json;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Download.Controllers;

public sealed class DatasetsControllerTests
{
    private static DatasetsController CreateController(
        Mock<IUseCase<DownloadDatasetsRequest, UseCaseResponse<DownloadDatasetsResponse>>> useCaseMock,
        Mock<IMapper<Dataset, DownloadedDatasetViewModel>> mapperMock)
    {
        DatasetsController controller = new(
            useCaseMock.Object,
            mapperMock.Object)
        {
            TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>())
        };

        return controller;
    }

    [Fact]
    public void Index_ReturnsIndexView()
    {
        Mock<IUseCase<DownloadDatasetsRequest, UseCaseResponse<DownloadDatasetsResponse>>> useCaseMock =
            DownloadDatasetsUseCaseTestDouble.Mock();

        Mock<IMapper<Dataset, DownloadedDatasetViewModel>> mapperMock =
            UseCaseReponseToViewModelMapperTestDouble.Mock();

        DatasetsController controller = CreateController(useCaseMock, mapperMock);

        IActionResult result = controller.Index();

        ViewResult view = Assert.IsType<ViewResult>(result);
        Assert.Equal("Index", view.ViewName);
    }

    [Fact]
    public async Task StartDownloading_RedirectsToDownloading()
    {
        Mock<IUseCase<DownloadDatasetsRequest, UseCaseResponse<DownloadDatasetsResponse>>> useCaseMock =
            DownloadDatasetsUseCaseTestDouble.Mock();

        Mock<IMapper<Dataset, DownloadedDatasetViewModel>> mapperMock =
            UseCaseReponseToViewModelMapperTestDouble.Mock();

        DatasetsController controller = CreateController(useCaseMock, mapperMock);

        IActionResult result = await controller.StartDownloading();

        RedirectToActionResult redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Downloading", redirect.ActionName);
    }

    [Fact]
    public void Downloading_ReturnsDownloadingView()
    {
        Mock<IUseCase<DownloadDatasetsRequest, UseCaseResponse<DownloadDatasetsResponse>>> useCaseMock =
            DownloadDatasetsUseCaseTestDouble.Mock();

        Mock<IMapper<Dataset, DownloadedDatasetViewModel>> mapperMock =
            UseCaseReponseToViewModelMapperTestDouble.Mock();

        DatasetsController controller = CreateController(useCaseMock, mapperMock);

        IActionResult result = controller.Downloading();

        ViewResult view = Assert.IsType<ViewResult>(result);
        Assert.Equal("Downloading", view.ViewName);
    }

    [Fact]
    public async Task FileDownload_ReturnsFileResult_AndStoresTempData()
    {
        Dataset dataset = new(
            Filename: "test-dataset",
            DataType: "All Establishments",
            DataFormat: "CSV",
            FileSize: 123,
            File: [1, 2, 3]
        );

        DownloadDatasetsResponse responseModel = new(dataset);

        UseCaseResponse<DownloadDatasetsResponse> useCaseResponse =
            UseCaseResponse<DownloadDatasetsResponse>.Success(responseModel);

        Mock<IUseCase<DownloadDatasetsRequest, UseCaseResponse<DownloadDatasetsResponse>>> useCaseMock =
            DownloadDatasetsUseCaseTestDouble.MockFor(useCaseResponse);

        DownloadedDatasetViewModel viewModel =
            new() {
                Filename = dataset.Filename!,
                FileSize = dataset.FileSize,
                FileFormat = dataset.DataFormat,
                ContentType = dataset.DataType
            };

        Mock<IMapper<Dataset, DownloadedDatasetViewModel>> mapperMock =
            UseCaseReponseToViewModelMapperTestDouble.MockFor(viewModel, dataset);

        DatasetsController controller = CreateController(useCaseMock, mapperMock);

        IActionResult result = await controller.FileDownload();

        FileContentResult fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("application/zip", fileResult.ContentType);
        Assert.Equal("test-dataset.zip", fileResult.FileDownloadName);
        Assert.Equal(dataset.File, fileResult.FileContents);

        Assert.True(controller.TempData.ContainsKey("DownloadedViewModel"));
    }

    [Fact]
    public void Status_ReturnsJsonReadyFalse_WhenNoTempData()
    {
        Mock<IUseCase<DownloadDatasetsRequest, UseCaseResponse<DownloadDatasetsResponse>>> useCaseMock =
            DownloadDatasetsUseCaseTestDouble.Mock();

        Mock<IMapper<Dataset, DownloadedDatasetViewModel>> mapperMock =
            UseCaseReponseToViewModelMapperTestDouble.Mock();

        DatasetsController controller = CreateController(useCaseMock, mapperMock);

        IActionResult result = controller.Status();

        JsonResult json = Assert.IsType<JsonResult>(result);
        Assert.False((bool)json.Value!.GetType().GetProperty("ready")!.GetValue(json.Value)!);
    }

    [Fact]
    public void Status_ReturnsJsonReadyTrue_WhenTempDataExists()
    {
        Mock<IUseCase<DownloadDatasetsRequest, UseCaseResponse<DownloadDatasetsResponse>>> useCaseMock =
            DownloadDatasetsUseCaseTestDouble.Mock();

        Mock<IMapper<Dataset, DownloadedDatasetViewModel>> mapperMock =
            UseCaseReponseToViewModelMapperTestDouble.Mock();

        DatasetsController controller = CreateController(useCaseMock, mapperMock);

        controller.TempData["DownloadedViewModel"] = "{}";

        IActionResult result = controller.Status();

        JsonResult json = Assert.IsType<JsonResult>(result);
        Assert.True((bool)json.Value!.GetType().GetProperty("ready")!.GetValue(json.Value)!);
    }

    [Fact]
    public void Complete_ReturnsCompleteView_WithDeserializedModel()
    {
        Mock<IUseCase<DownloadDatasetsRequest, UseCaseResponse<DownloadDatasetsResponse>>> useCaseMock =
            DownloadDatasetsUseCaseTestDouble.Mock();

        Mock<IMapper<Dataset, DownloadedDatasetViewModel>> mapperMock =
            UseCaseReponseToViewModelMapperTestDouble.Mock();

        DatasetsController controller = CreateController(useCaseMock, mapperMock);

        DownloadedDatasetViewModel viewModel =
            new() {
                Filename = "file",
                FileFormat = "All Establishments",
                ContentType = "CSV",
                FileSize = 123 };

        controller.TempData["DownloadedViewModel"] =
            JsonSerializer.Serialize(viewModel);

        IActionResult result = controller.Complete();

        ViewResult view = Assert.IsType<ViewResult>(result);
        Assert.Equal("Complete", view.ViewName);

        DownloadedDatasetViewModel model =
            Assert.IsType<DownloadedDatasetViewModel>(view.Model);

        Assert.Equal("file", model.Filename);
        Assert.Equal(123, model.FileSize);
    }
}
