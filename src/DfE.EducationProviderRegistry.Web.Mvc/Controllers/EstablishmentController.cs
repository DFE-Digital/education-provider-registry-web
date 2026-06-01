using DfE.EducationProviderRegistry.Web.Mvc.ViewModels.Pages;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Controllers;

public class EstablishmentController : Controller
{
    [HttpGet("/establishment/{urn}")]
    public IActionResult Details(string urn)
    {
        // Fake data for now
        var model = new EstablishmentDetailsPageViewModel
        {
            Urn = urn,
            Name = "Example School",
            Address = "123 Example Road",
            Type = "Academy",
            LocalAuthority = "Example LA",
            TrustName = "Example Trust"
        };

        return View(model);
    }
}