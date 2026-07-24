using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Sort;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
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
        UseCaseResponse<SearchResponse>,
        SearchResultsViewModel> _searchResponseToViewModelMapper;

    private readonly IMapper<
        Dictionary<string, List<string>>?,
        ReadOnlyCollection<FilterRequest>> _facetResultToViewModelMapper;

    public SearchController(
        IUseCase<SearchRequest, UseCaseResponse<SearchResponse>> searchUseCase,
        IMapper<UseCaseResponse<SearchResponse>, SearchResultsViewModel>
            searchResponseToViewModelMapper,
        IMapper<
            Dictionary<string, List<string>>?,
            ReadOnlyCollection<FilterRequest>> facetResultToViewModelMapper)
    {
        ArgumentNullException.ThrowIfNull(searchUseCase);
        ArgumentNullException.ThrowIfNull(searchResponseToViewModelMapper);
        ArgumentNullException.ThrowIfNull(facetResultToViewModelMapper);

        _searchUseCase = searchUseCase;
        _searchResponseToViewModelMapper =
            searchResponseToViewModelMapper;
        _facetResultToViewModelMapper =
            facetResultToViewModelMapper;
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
                searchResponse);

        updatedModel.PrimarySearchTerms =
            model.SearchKeywords!;

        updatedModel.SecondarySearchTerms =
            model.Address;

        updatedModel.SearchRequest = model;

        updatedModel.SearchFilters = BuildFilters(
            searchFilterRequests,
            model,
            searchResponse);

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

    private static SearchFiltersViewModel BuildFilters(
        ReadOnlyCollection<FilterRequest> requests,
        SearchRequestViewModel searchRequest,
        UseCaseResponse<SearchResponse> searchResponse)
    {
        Dictionary<string, FilterRequest> filtersByName =
            requests.ToDictionary(
                filter => filter.FilterName,
                StringComparer.OrdinalIgnoreCase);

        string? GetFirstValue(string filterName)
        {
            return filtersByName
                .GetValueOrDefault(filterName)?
                .FilterValues
                .FirstOrDefault()?
                .ToString();
        }

        IReadOnlyCollection<string> selectedEstablishmentTypes = [];

        if (searchRequest != null && searchRequest.SelectedFacets != null)
        {
            if (searchRequest.SelectedFacets.TryGetValue(
                    "EstablishmentType",
                    out var selectedValues))
            {
                selectedEstablishmentTypes = selectedValues;
            }
        }

        FilterViewModel[] filters =
        [
            new TextFilterViewModel
        {
            Name = nameof(SearchRequestViewModel.SearchKeywords),
            BindingName = nameof(SearchRequestViewModel.SearchKeywords),
            Label = "Establishment name or reference",
            Value = searchRequest.SearchKeywords
        },

        new AutocompleteFilterViewModel
        {
            Name = "LocalAuthority",
            BindingName = "SelectedFacets[LocalAuthority]",
            Label = "Local authority",
            Hint = "Start typing a local authority name",
            SelectedValue = GetFirstValue("LocalAuthority"),

            Options =
            [
                .. searchResponse.Model.EstablishmentResults.EstablishmentCollection
                    .Where(result =>
                        !string.IsNullOrWhiteSpace(
                            result.LocalAuthority.Name))
                    .Select(result =>
                        result.LocalAuthority.Name)
                    .Distinct(
                        StringComparer.OrdinalIgnoreCase)
                    .OrderBy(name => name)
                    .Select(name => new SelectListItem
                    {
                        Text = name,
                        Value = name
                    })
            ]
        },

        new CheckboxFilterViewModel
        {
            Name = "EstablishmentType",
            BindingName =
                "SelectedFacets[EstablishmentType]",
            Label = "Establishment type",

            Facet = new FacetViewModel(
                "EstablishmentType",
                [
                    .. searchResponse.Model.EstablishmentResults.EstablishmentCollection
                        .Where(result =>
                            !string.IsNullOrWhiteSpace(
                                result.Type.Value))
                        .GroupBy(
                            result => result.Type.Value!,
                            StringComparer.OrdinalIgnoreCase)
                        .OrderBy(group => group.Key)
                        .Select(group =>
                            new FacetValueViewModel(
                                group.Key,
                                group.Count(),
                                selectedEstablishmentTypes.Contains(
                                    group.Key,
                                    StringComparer.OrdinalIgnoreCase)))
                ])
        }
        ];

        SelectedFilterViewModel[] selectedFilters = filters
            .OfType<CheckboxFilterViewModel>()
            .SelectMany(filter =>
                filter.Facet.Values
                    .Where(value => value.IsSelected)
                    .Select(value =>
                        new SelectedFilterViewModel(
                            Label: value.Value,
                            BindingName: filter.BindingName,
                            Value: value.Value)))
            .Concat(
                filters
                    .OfType<AutocompleteFilterViewModel>()
                    .Where(filter =>
                        !string.IsNullOrWhiteSpace(
                            filter.SelectedValue))
                    .Select(filter =>
                        new SelectedFilterViewModel(
                            Label: filter.SelectedValue!,
                            BindingName: filter.BindingName,
                            Value: filter.SelectedValue!)))
            .ToArray();

        return new SearchFiltersViewModel
        {
            Filters = filters,
            SelectedFilters = selectedFilters
        };
    }
}