using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Shared.Filters;

public sealed class BreadcrumbTestController : Controller
{
    [HttpGet("/breadcrumb-tests/view")]
    public IActionResult ViewResultAction(string heading = "Test establishment")
    {
        BreadcrumbTestViewModel model = new()
        {
            Heading = heading
        };

        return View("BreadcrumbTest", model);
    }

    [HttpGet("/breadcrumb-tests/content")]
    public IActionResult ContentResultAction()
    {
        return Content("Content result");
    }

    [HttpGet("/breadcrumb-tests/redirect")]
    public IActionResult RedirectResultAction()
    {
        return Redirect("/breadcrumb-tests/view");
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
