using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Web.Mvc.ApplicationDtos;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;
using DfE.EducationProviderRegistry.Web.Mvc.ViewModels.Pages;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Controllers;

public class SearchController : Controller
{
    private readonly IMapper<EstablishmentSearchResultDto, GovUkTable> _searchResultsEstablishmentSummaryTableMapper;

    public SearchController(
        IMapper<EstablishmentSearchResultDto, GovUkTable> searchResultsMapper)
    {
        _searchResultsEstablishmentSummaryTableMapper = searchResultsMapper;
    }


    [HttpGet("/search")]
    public IActionResult Index()
    {
        return View(new SearchPageViewModel());
    }

    [HttpPost("/search")]
    public IActionResult Results(SearchPageViewModel model)
    {
        // Fake application DTOs for now
        List<EstablishmentSearchResultDto> results =
        [
            new() { Name = "St Mary's Primary", Urn = "123456", Type = "Academy", Address = "123 Example Road", LocalAuthorityName = "Birmingham", LocalAuthorityCode = "001", PartOfName = "Erdington Trust", PartOfCode = "001" },
            new() { Name = "St Mary's and St John's Cofe School", Urn = "123456", Type = "Independent", Address = "123 Example Road", LocalAuthorityName = "Birmingham", LocalAuthorityCode = "001" },
            new() { Name = "St Mary's Catholic School", Urn = "123456", Type = "Maintained", Address = "123 Example Road", LocalAuthorityName = "Birmingham", LocalAuthorityCode = "001" }
        ];

        return View(new SearchResultsPageViewModel
        {
            Query = model.Query,
            EstablishmentResults = results.Select(_searchResultsEstablishmentSummaryTableMapper.Map).ToList()
        });
    }
}
