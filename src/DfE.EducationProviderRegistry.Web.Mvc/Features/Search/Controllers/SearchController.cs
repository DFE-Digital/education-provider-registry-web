using System.Collections.ObjectModel;
using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Sort;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Services;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Controllers;

[Route("search")]
public sealed class SearchController : Controller
{
    private readonly IUseCase<SearchRequest, UseCaseResponse<SearchResponse>> _searchUseCase;
    private readonly IMapper<SearchResultsMappingContext, SearchResultsViewModel> _searchResponseToViewModelMapper;
    private readonly IMapper<SearchRequestViewModel, ReadOnlyCollection<FilterRequest>> _selectedFiltersToFiltersRequestsMapper;
    private readonly ISearchFilterSelectionHandler _searchFilterSelectionHandler;

    public SearchController(
        IUseCase<SearchRequest, UseCaseResponse<SearchResponse>> searchUseCase,
        IMapper<SearchResultsMappingContext, SearchResultsViewModel> searchResponseToViewModelMapper,
        IMapper<SearchRequestViewModel, ReadOnlyCollection<FilterRequest>> selectedFiltersToFiltersRequestsMapper,
        ISearchFilterSelectionHandler searchFilterSelectionHandler)
    {
        ArgumentNullException.ThrowIfNull(searchUseCase);
        ArgumentNullException.ThrowIfNull(searchResponseToViewModelMapper);
        ArgumentNullException.ThrowIfNull(selectedFiltersToFiltersRequestsMapper);
        ArgumentNullException.ThrowIfNull(searchFilterSelectionHandler);

        _searchUseCase = searchUseCase;
        _searchResponseToViewModelMapper = searchResponseToViewModelMapper;
        _selectedFiltersToFiltersRequestsMapper = selectedFiltersToFiltersRequestsMapper;
        _searchFilterSelectionHandler = searchFilterSelectionHandler;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return View("Index", new SearchRequestViewModel());
    }

    [HttpGet("results")]
    public async Task<IActionResult> Search(
        SearchRequestViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        string sortDirection = model.Sort switch
        {
            "za" => "desc",
            _ => "asc"
        };

        SortOrder sortOrder = new(
            sortField: "TO_BE_DEFINED",
            sortDirection: sortDirection,
            validSortFields:
            [
                "TO_BE_DEFINED"
            ]);

        _searchFilterSelectionHandler.Handle(model);

        ReadOnlyCollection<FilterRequest> searchFilterRequests =
            _selectedFiltersToFiltersRequestsMapper.Map(model);

        SearchRequest searchRequest = new(
            searchTerms: [
                new SearchTermRequest("what", model.What!),
                new SearchTermRequest("where", model.Where!),
            ],
            searchFilterRequests,
            sortOrder,
            model.Offset,
            model.RecordsPerPage);

        UseCaseResponse<SearchResponse> searchResponse =
            await _searchUseCase.HandleRequestAsync(
                searchRequest);

        SearchResultsViewModel updatedModel =
            _searchResponseToViewModelMapper.Map(
                new SearchResultsMappingContext(
                    model,
                    searchResponse));

        ModelState.Clear();

        return View("Results", updatedModel);
    }
}
