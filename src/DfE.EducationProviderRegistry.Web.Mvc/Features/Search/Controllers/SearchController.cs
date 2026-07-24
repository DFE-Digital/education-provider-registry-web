using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Sort;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.ObjectModel;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Controllers;

[Route("search")]
public sealed class SearchController : Controller
{
    private readonly IUseCase<
        SearchRequest,
        UseCaseResponse<SearchResponse>> _searchUseCase;

    private readonly IMapper<
        SearchResultsMappingContext,
        SearchResultsViewModel> _searchResponseToViewModelMapper;

    private readonly IMapper<
        Dictionary<string, List<string>>?,
        ReadOnlyCollection<FilterRequest>> _facetResultToViewModelMapper;

    private readonly IMapper<
        SearchFiltersMappingContext,
        SearchFiltersViewModel> _searchFiltersViewModelMapper;


    public SearchController(
        IUseCase<SearchRequest, UseCaseResponse<SearchResponse>> searchUseCase,
        IMapper<SearchResultsMappingContext, SearchResultsViewModel> searchResponseToViewModelMapper,
        IMapper<
            Dictionary<string, List<string>>?,
            ReadOnlyCollection<FilterRequest>> facetResultToViewModelMapper,
        IMapper<
            SearchFiltersMappingContext,
            SearchFiltersViewModel> searchFiltersViewModelMapper)
    {
        ArgumentNullException.ThrowIfNull(searchUseCase);
        ArgumentNullException.ThrowIfNull(searchResponseToViewModelMapper);
        ArgumentNullException.ThrowIfNull(facetResultToViewModelMapper);
        ArgumentNullException.ThrowIfNull(searchFiltersViewModelMapper);

        _searchUseCase = searchUseCase;
        _searchResponseToViewModelMapper =
            searchResponseToViewModelMapper;
        _facetResultToViewModelMapper =
            facetResultToViewModelMapper;
        _searchFiltersViewModelMapper =
            searchFiltersViewModelMapper;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return View("Index", new SearchRequestViewModel());
    }

    [HttpPost("")]
    public async Task<IActionResult> Search(
        SearchRequestViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        SortOrder sortOrder = new(
            sortField: "TO_BE_DEFINED",
            sortDirection: "ASC",
            validSortFields: new List<string>
            {
                "TO_BE_DEFINED"
            }.AsReadOnly());

        HandleFilterSelection(model);

        ReadOnlyCollection<FilterRequest> searchFilterRequests =
            _facetResultToViewModelMapper.Map(
                model.SelectedFacets);

        SearchRequest searchRequest = new(
            searchIndexKey: "TO_BE_REMOVED_FROM_CORE",
            searchKeywords: model.SearchKeywords!,
            searchFilterRequests,
            sortOrder);

        UseCaseResponse<SearchResponse> searchResponse =
            await _searchUseCase.HandleRequestAsync(
                searchRequest);

        SearchResultsViewModel updatedModel =
            _searchResponseToViewModelMapper.Map(
                new SearchResultsMappingContext(
                    model,
                    searchResponse));

        updatedModel.SearchFilters =
            _searchFiltersViewModelMapper.Map(
                new SearchFiltersMappingContext(
                    searchFilterRequests,
                    model,
                    searchResponse));

        ModelState.Clear();

        return View("Results", updatedModel);
    }

    private void HandleFilterSelection(
        SearchRequestViewModel model)
    {
        if (model.ClearFilters)
        {
            model.SelectedFacets.Clear();
        }

        if (!string.IsNullOrWhiteSpace(model.RemoveFilter))
        {
            var parts = model.RemoveFilter.Split('|', 2);

            if (parts.Length == 2)
            {
                var bindingName = parts[0];
                var value = parts[1];

                var facetName = bindingName
                    .Replace("SelectedFacets[", "")
                    .Replace("]", "");

                if (model.SelectedFacets.TryGetValue(facetName, out var values))
                {
                    values.Remove(value);

                    if (values.Count == 0)
                    {
                        model.SelectedFacets.Remove(facetName);
                    }
                }
            }
        }
    }
}