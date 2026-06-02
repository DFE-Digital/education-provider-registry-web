using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;
using DfE.EducationProviderRegistry.Web.Mvc.ViewModels.Components;
using DfE.EducationProviderRegistry.Web.Mvc.ViewModels.Pages;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Controllers;

public class SearchController : Controller
{
    [HttpGet("/search")]
    public IActionResult Index()
    {
        return View(new SearchPageViewModel());
    }

    [HttpPost("/search")]
    public IActionResult Results(SearchPageViewModel model)
    {
        // Fake results for now
        List<EstablishmentSearchResultViewModel> results = new List<EstablishmentSearchResultViewModel>
            {
                new EstablishmentSearchResultViewModel
                {
                    Name = "Example School",
                    Urn = "123456",
                    Type = "Academy",
                    Address = "123 Example Road",
                    LocalAuthorityName = "Example LA",
                    LaCode = "111",
                    PartOfName = "Example Trust",
                    PartOfCode = "111"
                }
            };

        return View(new SearchResultsPageViewModel
        {
            Query = model.Query,
            Results = results
        });
    }
}
