using DfE.EducationProviderRegistry.Web.Mvc.Features.Download.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Controllers;

[Route("download")]
public sealed class DownloadController : Controller
{
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
            return View(model);
        }

        return View(model);
    }

    [HttpGet("complete")]
    public IActionResult Complete()
    {
        return View();
    }
}
