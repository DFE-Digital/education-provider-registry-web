using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Controllers;

[Route("download")]
public sealed class DownloadController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return View("Index");
    }
}