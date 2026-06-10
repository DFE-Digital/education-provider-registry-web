using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Controllers
{
    public class PrivacyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
