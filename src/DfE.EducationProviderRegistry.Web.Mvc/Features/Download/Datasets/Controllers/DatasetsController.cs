using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Models;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Datasets.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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

    private const string DownloadedViewModelKey = "DownloadedViewModel";

    public DatasetsController(
        IUseCase<
            DownloadDatasetsRequest,
            UseCaseResponse<DownloadDatasetsResponse>> downloadDatasetUsecase,
        IMapper<
            Dataset,
            DownloadedDatasetViewModel> useCaseReponseToViewModelMapper)
    {
        _downloadDatasetUsecase = downloadDatasetUsecase;
        _useCaseReponseToViewModelMapper = useCaseReponseToViewModelMapper;
    }

    [HttpGet("")]
    public IActionResult Index() => View("Index");

    [HttpPost("downloading")]
    public async Task<IActionResult> StartDownloading() => 
        RedirectToAction("Downloading");

    [HttpGet("downloading")]
    public IActionResult Downloading() => View("Downloading");

    [HttpGet("file")]
    public async Task<IActionResult> FileDownload()
    {
        DownloadDatasetsRequest request = new(filename: string.Empty);

        UseCaseResponse<DownloadDatasetsResponse> response =
            await _downloadDatasetUsecase.HandleRequestAsync(request) ??
                throw new InvalidOperationException("Use case returned a null response.");
        
        DownloadDatasetsResponse model = response.Model
            ?? throw new InvalidOperationException("Use case response.Model was null.");

        Dataset dataset = model.DownloadedDataset
            ?? throw new InvalidOperationException("DownloadedDataset was null.");

        DownloadedDatasetViewModel viewModel =
            _useCaseReponseToViewModelMapper.Map(dataset);

        TempData[DownloadedViewModelKey] =
            JsonSerializer.Serialize(viewModel);

        return File(
            dataset.File,
            "application/zip",
            dataset.Filename + ".zip");
    }

    [HttpGet("status")]
    public IActionResult Status()
    {
        bool ready = TempData.ContainsKey(DownloadedViewModelKey);
        return Json(new { ready });
    }

    [HttpGet("complete")]
    public IActionResult Complete()
    {
        string jsonViewModel =
            TempData.Peek(DownloadedViewModelKey)!.ToString()!;
        
        DownloadedDatasetViewModel viewModel =
            JsonSerializer.Deserialize<DownloadedDatasetViewModel>(jsonViewModel)!;

        return View("Complete", viewModel);
    }
}
