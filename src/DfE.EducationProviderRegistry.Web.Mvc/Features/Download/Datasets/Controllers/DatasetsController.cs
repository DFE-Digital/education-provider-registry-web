using System.Text.Json;
using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Models;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Datasets.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Datasets.Controllers;

[Route("datasets")]
public sealed class DatasetsController : Controller
{
    private readonly IUseCase<
        DownloadDatasetsRequest,
        UseCaseResponse<DownloadDatasetsResponse>> _downloadDatasetUsecase;

    private readonly IMapper<
        Dataset,
        DownloadedDatasetViewModel> _useCaseReponseToViewModelMapper;

    private readonly ISession _sessionContext;

    private const string DownloadedViewModelKey = "DownloadedViewModel";

    public DatasetsController(
        IUseCase<
            DownloadDatasetsRequest,
            UseCaseResponse<DownloadDatasetsResponse>> downloadDatasetUsecase,
        IMapper<
            Dataset,
            DownloadedDatasetViewModel> useCaseReponseToViewModelMapper,
        ISession sessionContext)
    {
        _downloadDatasetUsecase = downloadDatasetUsecase;
        _useCaseReponseToViewModelMapper = useCaseReponseToViewModelMapper;
        _sessionContext = sessionContext;
    }

    [HttpGet("")]
    public IActionResult Index() => View(nameof(Index));

    [HttpPost("downloading")]
    public IActionResult StartDownloading(
        DownloadDatasetRequestViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(nameof(Index), viewModel);
        }

        _sessionContext.Remove(DownloadedViewModelKey);

        return RedirectToAction(nameof(Downloading), viewModel);
    }

    [HttpGet("downloading")]
    public IActionResult Downloading(
        DownloadDatasetRequestViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(nameof(Index), viewModel);
        }

        return View(nameof(Downloading), viewModel);
    }

    [HttpGet("file")]
    public async Task<IActionResult> FileDownload(string filename)
    {
        DownloadDatasetsRequest request = new(filename);

        UseCaseResponse<DownloadDatasetsResponse> response =
            await _downloadDatasetUsecase.HandleRequestAsync(request)
            ?? throw new InvalidOperationException(
                "Use case returned a null response.");

        DownloadDatasetsResponse model =
            response.Model
                ?? throw new InvalidOperationException(
                    "Use case response.Model was null.");

        Dataset dataset =
            model.DownloadedDataset
                ?? throw new InvalidOperationException(
                    "DownloadedDataset was null.");

        DownloadedDatasetViewModel viewModel =
            _useCaseReponseToViewModelMapper.Map(dataset);

        _sessionContext.SetString(
            DownloadedViewModelKey,
            JsonSerializer.Serialize(viewModel));

        return File(
            dataset.FileStream,
            "application/zip",
            viewModel.ZipFilename,
            enableRangeProcessing: true);
    }

    [HttpGet("status")]
    public IActionResult Status()
    {
        bool ready =
            !string.IsNullOrWhiteSpace(
                _sessionContext.GetString(DownloadedViewModelKey));

        return Json(new { ready });
    }

    [HttpGet("complete")]
    public IActionResult Complete()
    {
        string? jsonViewModel =
            _sessionContext.GetString(DownloadedViewModelKey);

        if (string.IsNullOrWhiteSpace(jsonViewModel))
        {
            return RedirectToAction(nameof(Index));
        }

        DownloadedDatasetViewModel viewModel =
            JsonSerializer.Deserialize<DownloadedDatasetViewModel>(
                jsonViewModel)!;

        _sessionContext.Remove(DownloadedViewModelKey);

        return View(nameof(Complete), viewModel);
    }
}
