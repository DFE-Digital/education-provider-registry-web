using DfE.EducationProviderRegistry.Web.Mvc.Features.Download.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Controllers;

[Route("download")]
public sealed class DownloadController : Controller
{
    private static readonly Dictionary<string, DateTime> _jobStartTimes = [];

    [HttpGet]
    public IActionResult Index()
    {
        return View("Index");
    }

    [AcceptVerbs("GET", "POST")]
    [Route("download/downloading")]
    public IActionResult Downloading([FromForm] DownloadViewModel? model)
    {
        if (HttpContext.Request.Method == "POST")
        {
            string? key = User.Identity?.Name ?? "anon";
            _jobStartTimes[key] = DateTime.UtcNow;

            return View(model);
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
    public IActionResult Complete()
    {
        return View();
    }
}
