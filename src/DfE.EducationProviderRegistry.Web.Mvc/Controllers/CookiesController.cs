using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Controllers;

public class CookiesController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
