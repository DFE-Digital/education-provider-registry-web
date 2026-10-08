using System.Diagnostics;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Breadcrumbs.Services;
using DfE.EducationProviderRegistry.Web.Mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly IBreadcrumbJourneyService _navigation;
    public HomeController(IBreadcrumbJourneyService navigation)
    {
        ArgumentNullException.ThrowIfNull(navigation);
        _navigation = navigation;
    }

    public IActionResult Index()
    {
        _navigation.Clear();
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
