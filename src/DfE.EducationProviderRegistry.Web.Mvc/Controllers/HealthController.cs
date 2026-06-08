using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Controllers
{
    public class HealthController : Controller
    {
        public IActionResult Index()
        {
            return Ok(new
            {
                status = "Service is running",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
