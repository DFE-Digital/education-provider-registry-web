using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.DownloadDatasets.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.DownloadDatasets.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Web.Mvc.Features.DownloadDatasets.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.DownloadDatasets.Controllers;

[Route("downloaddatasets")]
public sealed class DownloadDatasetsController : Controller
{
    private static readonly Dictionary<string, DateTime> _jobStartTimes = [];

    private readonly IUseCase<
        DownloadDatasetsRequest,
        UseCaseResponse<DownloadDatasetsResponse>> _downloadDatasetUsecase;

    private readonly IMapper<
        DownloadDatasetsResponse,
        DownloadedDatasetViewModel> _usecaseResponseToViewModelMapper;

    public DownloadDatasetsController(
        IUseCase<
            DownloadDatasetsRequest,
            UseCaseResponse<DownloadDatasetsResponse>> downloadDatasetUsecase,
        IMapper<
        DownloadDatasetsResponse,
        DownloadedDatasetViewModel> usecaseResponseToViewModelMapper)
    {
        _downloadDatasetUsecase = downloadDatasetUsecase;
        _usecaseResponseToViewModelMapper = usecaseResponseToViewModelMapper;
    }

    [HttpGet]
    public IActionResult Index() => View("Index");

    [AcceptVerbs("GET", "POST")]
    [Route("downloaddatasets/downloading")]
    public async Task<IActionResult> Downloading([FromForm] DownloadDatasetRequestViewModel? model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        if (HttpContext.Request.Method == "POST")
        {
            string? key = User.Identity?.Name ?? "anon";
            _jobStartTimes[key] = DateTime.UtcNow;

            DownloadDatasetsRequest request = new(filename: "");

            UseCaseResponse<DownloadDatasetsResponse> response =
                await _downloadDatasetUsecase.HandleRequestAsync(request);

            DownloadedDatasetViewModel viewModel =
                _usecaseResponseToViewModelMapper.Map(response.Model);

            return View(viewModel);
        }

        return View(model);
    }

    [HttpGet("status")]
    public IActionResult Status()
    {
        string? key = User.Identity?.Name ?? "anon";

        if (!_jobStartTimes.TryGetValue(key, out var started))
        {
            return Json(new { ready = false });
        }

        TimeSpan elapsed = DateTime.UtcNow - started;
        bool isReady = elapsed.TotalSeconds >= 5;

        return Json(new { ready = isReady });
    }

    [HttpGet("complete")]
    public IActionResult Complete() => View();

    [HttpGet("file")]
    public IActionResult FakeFileDownload()
    {
        var csv = new StringBuilder();

        csv.AppendLine("UPN,Forename,Surname");
        csv.AppendLine("123456789012,John,Smith");
        csv.AppendLine("987654321098,Sarah,Jones");
        csv.AppendLine("555555555555,Michael,Brown");

        byte[] bytes = Encoding.UTF8.GetBytes(csv.ToString());

        return File(bytes, "text/csv", "all-establishment-data.csv");
    }
}
