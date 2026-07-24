using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;

public sealed class SearchResponseToSearchFiltersViewModelMapper :
    IMapper<SearchFiltersMappingContext, SearchFiltersViewModel>
{
    public SearchFiltersViewModel Map(
        SearchFiltersMappingContext source)
    {
        ArgumentNullException.ThrowIfNull(source);

        Dictionary<string, FilterRequest> filtersByName =
            source.FilterRequests.ToDictionary(
                filter => filter.FilterName,
                StringComparer.OrdinalIgnoreCase);

        string? selectedLocalAuthority =
            GetFirstValue(
                filtersByName,
                "LocalAuthority");

        IReadOnlyCollection<string> selectedEstablishmentTypes =
            GetSelectedValues(
                source.SearchRequest,
                "EstablishmentType");

        FilterViewModel[] filters =
        [
            BuildLocalAuthorityFilter(
                source.SearchResponse,
                selectedLocalAuthority),

            BuildEstablishmentTypeFilter(
                source.SearchResponse,
                selectedEstablishmentTypes)
        ];

        return new SearchFiltersViewModel
        {
            Filters = filters,
            SelectedFilters = BuildSelectedFilters(filters)
        };
    }

    private static string? GetFirstValue(
        IReadOnlyDictionary<string, FilterRequest> filters,
        string filterName)
    {
        return filters
            .GetValueOrDefault(filterName)?
            .FilterValues
            .FirstOrDefault()?
            .ToString();
    }

    private static IReadOnlyCollection<string> GetSelectedValues(
        SearchRequestViewModel searchRequest,
        string facetName)
    {
        if (searchRequest.SelectedFacets.TryGetValue(
                facetName,
                out List<string>? selectedValues))
        {
            return selectedValues;
        }

        return [];
    }

    private static AutocompleteFilterViewModel BuildLocalAuthorityFilter(
        UseCaseResponse<
            SearchResponse>
                searchResponse,
        string? selectedValue)
    {
        return new AutocompleteFilterViewModel
        {
            Name = "LocalAuthority",
            BindingName = "SelectedFacets[LocalAuthority]",
            Label = "Local authority",
            Hint = "Start typing a local authority name",
            SelectedValue = selectedValue,

            Options =
            [
                .. searchResponse.Model.EstablishmentResults
                    .EstablishmentCollection
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
        };
    }

    private static CheckboxFilterViewModel BuildEstablishmentTypeFilter(
        UseCaseResponse<
            SearchResponse>
                searchResponse,
        IReadOnlyCollection<string> selectedValues)
    {
        return new CheckboxFilterViewModel
        {
            Name = "EstablishmentType",
            BindingName =
                "SelectedFacets[EstablishmentType]",
            Label = "Establishment type",

            Facet = new FacetViewModel(
                "EstablishmentType",
                [
                    .. searchResponse.Model.EstablishmentResults
                        .EstablishmentCollection
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
                                selectedValues.Contains(
                                    group.Key,
                                    StringComparer.OrdinalIgnoreCase)))
                ])
        };
    }

    private static SelectedFilterViewModel[] BuildSelectedFilters(
        IEnumerable<FilterViewModel> filters)
    {
        return filters
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
    }
}