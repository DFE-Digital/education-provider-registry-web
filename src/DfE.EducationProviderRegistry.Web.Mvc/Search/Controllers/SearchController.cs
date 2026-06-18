using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Sort;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Web.Mvc.Search.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Controllers;

public sealed class SearchController : Controller
{
    private readonly IUseCase<SearchRequest, UseCaseResponse<SearchResponse>> _searchUseCase;
    private readonly IMapper<
        EstablishmentSearchResults, SearchResultsViewModel> _searchResponseToViewModelMapper;

    public SearchController(
        IUseCase<SearchRequest, UseCaseResponse<SearchResponse>> searchUseCase,
        IMapper<EstablishmentSearchResults, SearchResultsViewModel> searchResponseToViewModelMapper)
    {
        _searchUseCase = searchUseCase;
        _searchResponseToViewModelMapper = searchResponseToViewModelMapper;
    }

    [HttpGet("/search")]
    public IActionResult Index() => View("~/Search/Views/Index.cshtml", new SearchRequestViewModel());

    [HttpPost("/search")]
    public async Task<IActionResult> Results(SearchRequestViewModel model)
    {
        SortOrder sortOrder =
            new(
                sortField: "TO_BE_DEFINED",
                sortDirection: "ASC",
                validSortFields: new List<string>() { "TO_BE_DEFINED" }.AsReadOnly());

        SearchRequest searchRequest =
            new(searchIndexKey: "TO_BE_DEFINED", searchKeywords: "TO_BE_DEFINED", sortOrder);

        UseCaseResponse<SearchResponse> searchResponse =
            await _searchUseCase.HandleRequestAsync(searchRequest);

        EstablishmentSearchResults establishmentSearchResults =
            searchResponse.Model.EstablishmentResults;

        SearchResultsViewModel updatedModel =
            _searchResponseToViewModelMapper.Map(establishmentSearchResults);

        return View("~/Search/Views/Results.cshtml", updatedModel);
    }
}