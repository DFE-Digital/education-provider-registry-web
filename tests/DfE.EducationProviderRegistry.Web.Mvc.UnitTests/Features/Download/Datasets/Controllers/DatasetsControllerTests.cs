using System.Text.Json;
using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Models;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Datasets.Controllers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Datasets.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Download.Datasets.Controllers.TestDoubles;
using DfE.EducationProviderRegistry.Web.Mvc.UnitTests.TestDoubles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Download.Datasets.Controllers;

public sealed class DatasetsControllerTests
{
    private static DatasetsController CreateController(
        Mock<IUseCase<DownloadDatasetsRequest, UseCaseResponse<DownloadDatasetsResponse>>> useCaseMock,
        Mock<IMapper<Dataset, DownloadedDatasetViewModel>> mapperMock,
        ISession session)
    {
        DatasetsController controller =
            new(
                useCaseMock.Object,
                mapperMock.Object,
                session
            )
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

        return controller;
    }

    [Fact]
    public void Index_ReturnsIndexView()
    {
        // arrange
        Mock<IUseCase<DownloadDatasetsRequest, UseCaseResponse<DownloadDatasetsResponse>>> useCaseMock =
            DownloadDatasetsUseCaseTestDouble.Mock();

        Mock<IMapper<Dataset, DownloadedDatasetViewModel>> mapperMock =
            UseCaseReponseToViewModelMapperTestDouble.Mock();

        ISession session =
            SessionTestDouble.Create();

        DatasetsController controller =
            CreateController(useCaseMock, mapperMock, session);

        // act
        IActionResult result = controller.Index();

        // asssert
        ViewResult view = Assert.IsType<ViewResult>(result);
        Assert.Equal("Index", view.ViewName);
    }

    [Fact]
    public async Task StartDownloading_RedirectsToDownloading()
    {
        // arrange
        Mock<IUseCase<DownloadDatasetsRequest, UseCaseResponse<DownloadDatasetsResponse>>> useCaseMock =
            DownloadDatasetsUseCaseTestDouble.Mock();

        Mock<IMapper<Dataset, DownloadedDatasetViewModel>> mapperMock =
            UseCaseReponseToViewModelMapperTestDouble.Mock();

        ISession session =
            SessionTestDouble.Create();

        DatasetsController controller =
            CreateController(useCaseMock, mapperMock, session);

        // act
        DownloadDatasetRequestViewModel viewModel =
            new()
            {
                Filename = "file",
            };

        IActionResult result = controller.StartDownloading(viewModel);

        // assert
        RedirectToActionResult redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Downloading", redirect.ActionName);
    }

    [Fact]
    public void Downloading_ReturnsDownloadingView()
    {
        // arrange
        Mock<IUseCase<DownloadDatasetsRequest, UseCaseResponse<DownloadDatasetsResponse>>> useCaseMock =
            DownloadDatasetsUseCaseTestDouble.Mock();

        Mock<IMapper<Dataset, DownloadedDatasetViewModel>> mapperMock =
            UseCaseReponseToViewModelMapperTestDouble.Mock();

        ISession session =
            SessionTestDouble.Create();

        DatasetsController controller =
            CreateController(useCaseMock, mapperMock, session);

        // act
        DownloadDatasetRequestViewModel viewModel =
            new()
            {
                Filename = "file",
            };

        IActionResult result = controller.Downloading(viewModel);

        // assert
        ViewResult view = Assert.IsType<ViewResult>(result);
        Assert.Equal("Downloading", view.ViewName);
    }

    [Fact]
    public async Task FileDownload_ReturnsFileResult_AndStoresTempData()
    {
        // arrange
        byte[] fileBytes = [1, 2, 3];

        Dataset dataset =
            new(
                Filename: "test-dataset",
                DataType: "All Establishments",
                DataFormat: "CSV",
                FileSize: fileBytes.Length,
                FileStream: new MemoryStream(fileBytes)
            );

        DownloadDatasetsResponse responseModel = new(dataset);

        UseCaseResponse<DownloadDatasetsResponse> useCaseResponse =
            UseCaseResponse<DownloadDatasetsResponse>.Success(responseModel);

        Mock<IUseCase<DownloadDatasetsRequest, UseCaseResponse<DownloadDatasetsResponse>>> useCaseMock =
            DownloadDatasetsUseCaseTestDouble.MockFor(useCaseResponse);

        DownloadedDatasetViewModel viewModel =
            new()
            {
                Filename = dataset.Filename!,
                FileSize = dataset.FileSize,
                FileFormat = dataset.DataFormat,
                ContentType = dataset.DataType
            };

        Mock<IMapper<Dataset, DownloadedDatasetViewModel>> mapperMock =
            UseCaseReponseToViewModelMapperTestDouble.MockFor(viewModel, dataset);

        ISession session =
            SessionTestDouble.Create();

        DatasetsController controller =
            CreateController(useCaseMock, mapperMock, session);

        // act
        const string DatasetFileName = "test-dataset";
        IActionResult result = await controller.FileDownload(DatasetFileName);

        // assert
        FileStreamResult fileResult = Assert.IsType<FileStreamResult>(result);
        Assert.Equal("application/zip", fileResult.ContentType);
        Assert.Contains($"{DatasetFileName}", fileResult.FileDownloadName);

        using (MemoryStream memoryStream = new())
        {
            dataset.FileStream.CopyTo(memoryStream);
            byte[] byteArray = memoryStream.ToArray();

            Assert.Equal(byteArray, fileBytes);
        }

        Assert.NotNull(session.Get("DownloadedViewModel"));
    }

    [Fact]
    public void Status_ReturnsJsonReadyFalse_WhenNoTempData()
    {
        // arrange
        Mock<IUseCase<DownloadDatasetsRequest, UseCaseResponse<DownloadDatasetsResponse>>> useCaseMock =
            DownloadDatasetsUseCaseTestDouble.Mock();

        Mock<IMapper<Dataset, DownloadedDatasetViewModel>> mapperMock =
            UseCaseReponseToViewModelMapperTestDouble.Mock();

        ISession session =
            SessionTestDouble.Create();

        DatasetsController controller =
            CreateController(useCaseMock, mapperMock, session);

        // act
        IActionResult result = controller.Status();

        // assert
        JsonResult json = Assert.IsType<JsonResult>(result);
        Assert.False((bool)json.Value!.GetType().GetProperty("ready")!.GetValue(json.Value)!);
    }

    [Fact]
    public void Status_ReturnsJsonReadyTrue_WhenTempDataExists()
    {
        // arrange
        Mock<IUseCase<DownloadDatasetsRequest, UseCaseResponse<DownloadDatasetsResponse>>> useCaseMock =
            DownloadDatasetsUseCaseTestDouble.Mock();

        Mock<IMapper<Dataset, DownloadedDatasetViewModel>> mapperMock =
            UseCaseReponseToViewModelMapperTestDouble.Mock();

        ISession session =
            SessionTestDouble.Create();

        DatasetsController controller =
            CreateController(useCaseMock, mapperMock, session);

        session.SetString("DownloadedViewModel", "{}");

        // act
        IActionResult result = controller.Status();

        // assert
        JsonResult json = Assert.IsType<JsonResult>(result);
        Assert.True((bool)json.Value!.GetType().GetProperty("ready")!.GetValue(json.Value)!);
    }

    [Fact]
    public void Complete_ReturnsCompleteView_WithDeserializedModel()
    {
        // arrange
        Mock<IUseCase<DownloadDatasetsRequest, UseCaseResponse<DownloadDatasetsResponse>>> useCaseMock =
            DownloadDatasetsUseCaseTestDouble.Mock();

        Mock<IMapper<Dataset, DownloadedDatasetViewModel>> mapperMock =
            UseCaseReponseToViewModelMapperTestDouble.Mock();

        ISession session =
            SessionTestDouble.Create();

        DatasetsController controller =
            CreateController(useCaseMock, mapperMock, session);

        DownloadedDatasetViewModel viewModel =
            new()
            {
                Filename = "file",
                FileFormat = "All Establishments",
                ContentType = "CSV",
                FileSize = 123
            };

        session.SetString("DownloadedViewModel", JsonSerializer.Serialize(viewModel));

        // act
        IActionResult result = controller.Complete();

        // assert
        ViewResult view = Assert.IsType<ViewResult>(result);
        Assert.Equal("Complete", view.ViewName);

        DownloadedDatasetViewModel model =
            Assert.IsType<DownloadedDatasetViewModel>(view.Model);

        Assert.Equal("file", model.Filename);
        Assert.Equal(123, model.FileSize);
    }

    [Fact]
    public void StartDownloading_ReturnsIndexView_WhenModelStateIsInvalid()
    {
        // arrange
        Mock<IUseCase<
            DownloadDatasetsRequest,
            UseCaseResponse<DownloadDatasetsResponse>>> useCaseMock =
            DownloadDatasetsUseCaseTestDouble.Mock();

        Mock<IMapper<Dataset, DownloadedDatasetViewModel>> mapperMock =
            UseCaseReponseToViewModelMapperTestDouble.Mock();

        ISession session =
            SessionTestDouble.Create();

        DatasetsController controller =
            CreateController(useCaseMock, mapperMock, session);

        controller.ModelState.AddModelError("Filename", "The Filename field is required.");

        DownloadDatasetRequestViewModel viewModel = new();

        // act
        IActionResult result = controller.StartDownloading(viewModel);

        // assert
        ViewResult view = Assert.IsType<ViewResult>(result);
        Assert.Equal("Index", view.ViewName);
        Assert.Equal(viewModel, view.Model);
    }

    [Fact]
    public void Downloading_ReturnsIndexView_WhenModelStateIsInvalid()
    {
        // arrange
        Mock<IUseCase<
            DownloadDatasetsRequest,
            UseCaseResponse<DownloadDatasetsResponse>>> useCaseMock =
            DownloadDatasetsUseCaseTestDouble.Mock();

        Mock<IMapper<Dataset, DownloadedDatasetViewModel>> mapperMock =
            UseCaseReponseToViewModelMapperTestDouble.Mock();

        ISession session =
            SessionTestDouble.Create();

        DatasetsController controller =
            CreateController(useCaseMock, mapperMock, session);

        controller.ModelState.AddModelError("Filename", "The Filename field is required.");

        DownloadDatasetRequestViewModel viewModel = new();

        // act
        IActionResult result = controller.Downloading(viewModel);

        // assert
        ViewResult view = Assert.IsType<ViewResult>(result);
        Assert.Equal("Index", view.ViewName);
        Assert.Equal(viewModel, view.Model);
    }

    [Fact]
    public void Complete_RedirectsToIndex_WhenTempDataIsEmpty()
    {
        // arrange
        Mock<IUseCase<
            DownloadDatasetsRequest,
            UseCaseResponse<DownloadDatasetsResponse>>> useCaseMock =
            DownloadDatasetsUseCaseTestDouble.Mock();

        Mock<IMapper<Dataset, DownloadedDatasetViewModel>> mapperMock =
            UseCaseReponseToViewModelMapperTestDouble.Mock();

        ISession session =
            SessionTestDouble.Create();

        DatasetsController controller =
            CreateController(useCaseMock, mapperMock, session);

        // act
        IActionResult result = controller.Complete();

        // assert
        RedirectToActionResult redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }
}
