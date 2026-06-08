using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Controllers
{
    public class AccessibilityController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
