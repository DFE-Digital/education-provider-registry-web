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

/// <summary>
/// Handles search-related HTTP requests, including rendering the search page
/// and executing search operations against the underlying search use case.
/// </summary>
public sealed class SearchController : Controller
{
    private readonly IUseCase<SearchRequest, UseCaseResponse<SearchResponse>> _searchUseCase;
    private readonly IMapper<UseCaseResponse<SearchResponse>, SearchResultsViewModel> _searchResponseToViewModelMapper;
    private readonly IMapper<Dictionary<string, List<string>>?, ReadOnlyCollection<FilterRequest>> _facetResultToViewModelMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchController"/> class.
    /// </summary>
    /// <param name="searchUseCase">
    /// The use case responsible for executing search operations.
    /// </param>
    /// <param name="searchResponseToViewModelMapper">
    /// Maps the search use case response into a <see cref="SearchResultsViewModel"/> suitable for rendering.
    /// </param>
    /// <param name="facetResultToViewModelMapper">
    /// Maps selected facet values into a collection of <see cref="FilterRequest"/> objects.
    /// </param>
    public SearchController(
        IUseCase<SearchRequest, UseCaseResponse<SearchResponse>> searchUseCase,
        IMapper<UseCaseResponse<SearchResponse>, SearchResultsViewModel> searchResponseToViewModelMapper,
        IMapper<Dictionary<string, List<string>>?, ReadOnlyCollection<FilterRequest>> facetResultToViewModelMapper)
    {
        _searchUseCase = searchUseCase;
        _searchResponseToViewModelMapper = searchResponseToViewModelMapper;
        _facetResultToViewModelMapper = facetResultToViewModelMapper;
    }

    /// <summary>
    /// Renders the search page with an empty <see cref="SearchRequestViewModel"/>.
    /// </summary>
    /// <returns>
    /// A view containing the search form.
    /// </returns>
    [HttpGet("/search")]
    public IActionResult Index() =>
        View("~/Features/Search/Views/Index.cshtml", new SearchRequestViewModel());

    /// <summary>
    /// Executes a search operation using the submitted <see cref="SearchRequestViewModel"/>.
    /// </summary>
    /// <param name="model">
    /// The view model containing search keywords, selected facets, and other search parameters.
    /// </param>
    /// <returns>
    /// A view containing the search results, or the search form again if validation fails.
    /// </returns>
    [HttpPost("/search")]
    public async Task<IActionResult> Search(SearchRequestViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Features/Search/Views/Index.cshtml", model);
        }

        SortOrder sortOrder =
            new(
                sortField: "TO_BE_DEFINED",
                sortDirection: "ASC",
                validSortFields: new List<string> { "TO_BE_DEFINED" }.AsReadOnly());

        ReadOnlyCollection<FilterRequest> searchFilterRequests =
            _facetResultToViewModelMapper.Map(model.SelectedFacets);

        SearchRequest searchRequest =
            new(
                searchIndexKey: "TO_BE_REMOVED_FROM_CORE",
                searchKeywords: model.SearchKeywords!,
                searchFilterRequests,
                sortOrder);

        UseCaseResponse<SearchResponse> searchResponse =
            await _searchUseCase.HandleRequestAsync(searchRequest);

        SearchResultsViewModel updatedModel =
            _searchResponseToViewModelMapper.Map(searchResponse);

        updatedModel.PrimarySearchTerms = model.SearchKeywords!;

        return View("~/Features/Search/Views/Results.cshtml", updatedModel);
    }
}