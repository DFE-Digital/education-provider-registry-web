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
                    Name = "St Mary's and St John's Cofe School",
                    Urn = "123456",
                    Type = "Independant",
                    Address = "123 Example Road",
                    LocalAuthorityName = "Birminghan",
                    LocalAuthorityCode = "001",
                },
                new EstablishmentSearchResultViewModel
                {
                    Name = "St Mary's Primary",
                    Urn = "123456",
                    Type = "Academy",
                    Address = "123 Example Road",
                    LocalAuthorityName = "Birminghan",
                    LocalAuthorityCode = "001",
                    PartOfName = "Erdington Trust",
                    PartOfCode = "001"
                },
                new EstablishmentSearchResultViewModel
                {
                    Name = "St Mary's Catholic School",
                    Urn = "123456",
                    Type = "Maintained",
                    Address = "123 Example Road",
                    LocalAuthorityName = "Birminghan",
                    LocalAuthorityCode = "001",
                }
            };

        return View(new SearchResultsPageViewModel
        {
            Query = model.Query,
            Results = results
        });
    }
}
