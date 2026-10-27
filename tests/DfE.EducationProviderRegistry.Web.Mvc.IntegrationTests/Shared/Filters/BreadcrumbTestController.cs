using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Shared.Filters;

[Route("breadcrumb-tests")]
public sealed class BreadcrumbTestController : Controller
{
    [HttpGet("view")]
    public IActionResult ViewResultAction(string heading = "Test establishment")
    {
        BreadcrumbTestViewModel model = new()
        {
            Heading = heading
        };

        return View("BreadcrumbTest", model);
    }

    [HttpGet("redirect")]
    public IActionResult RedirectResultAction()
    {
        return Redirect("/breadcrumb-tests/view");
    }

    [HttpGet("session")]
    public IActionResult Session()
    {
        byte[]? journey = HttpContext.Session.Get("BreadcrumbJourney");

        return journey is null
            ? NotFound()
            : Ok(journey);
    }
}

public sealed class BreadcrumbTestViewModel
{
    public required string Heading { get; init; }
}


public sealed class NoOpViewResultExecutor
    : IActionResultExecutor<ViewResult>
{
    public Task ExecuteAsync(
        ActionContext context,
        ViewResult result)
    {
        return Task.CompletedTask;
    }
}
