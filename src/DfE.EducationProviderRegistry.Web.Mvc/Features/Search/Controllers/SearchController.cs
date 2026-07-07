using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Sort;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.ObjectModel;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Controllers;

public sealed class SearchController : Controller
{
    private readonly IUseCase<SearchRequest, UseCaseResponse<SearchResponse>> _searchUseCase;
    private readonly IMapper<
        UseCaseResponse<SearchResponse>, SearchResultsViewModel> _searchResponseToViewModelMapper;
    private readonly IMapper<
        Dictionary<string, List<string>>?, ReadOnlyCollection<FilterRequest>> _facetResultToViewModelMapper;

    public SearchController(
        IUseCase<SearchRequest, UseCaseResponse<SearchResponse>> searchUseCase,
        IMapper<
            UseCaseResponse<SearchResponse>, SearchResultsViewModel> searchResponseToViewModelMapper,
        IMapper<
            Dictionary<string, List<string>>?, ReadOnlyCollection<FilterRequest>> facetResultToViewModelMapper)
    {
        _searchUseCase = searchUseCase;
        _searchResponseToViewModelMapper = searchResponseToViewModelMapper;
        _facetResultToViewModelMapper = facetResultToViewModelMapper;
    }

    [HttpGet("/search")]
    public IActionResult Index() => View("~/Features/Search/Views/Index.cshtml", new SearchRequestViewModel());

    [HttpPost("/search")]
    public async Task<IActionResult> Search(SearchRequestViewModel model)
    {
        SortOrder sortOrder =
            new(
                sortField: "TO_BE_DEFINED",
                sortDirection: "ASC",
                validSortFields: new List<string>() { "TO_BE_DEFINED" }.AsReadOnly());

        ReadOnlyCollection<FilterRequest> searchFilterRequests =
            _facetResultToViewModelMapper.Map(model.SelectedFacets);

        SearchRequest searchRequest =
            new(
                searchIndexKey: "TO_BE_REMOVED_FROM_CORE",
                searchKeywords: model.SearchKeywords!,
                searchFilterRequests, sortOrder);

        UseCaseResponse<SearchResponse> searchResponse =
            await _searchUseCase.HandleRequestAsync(searchRequest);

        SearchResultsViewModel updatedModel =
            _searchResponseToViewModelMapper.Map(searchResponse);

        updatedModel.PrimarySearchTerms = model.SearchKeywords!;

        return View("~/Features/Search/Views/Results.cshtml", updatedModel);
    }
}