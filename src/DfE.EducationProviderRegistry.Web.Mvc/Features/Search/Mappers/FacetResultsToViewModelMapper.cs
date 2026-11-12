using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.Filters;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;

public sealed record FacetResultsMappingContext(
    IReadOnlyCollection<SearchFacet> Facets,
    Dictionary<string, List<string>>? SelectedFacets);

public sealed class FacetResultsToViewModelMapper : IMapper<FacetResultsMappingContext, IReadOnlyCollection<IFilterViewModel>>
{
    private static readonly Dictionary<string, string> _labelMappings = new()
    {
        ["admissionpolicy"] = "Admissions policy",
        ["boarding"] = "Boarding provision",
        ["diocese"] = "Diocese",
        ["educationphase"] = "Phase of education",
        ["furthereducationtype"] = "Further education type",
        ["gender"] = "Gender",
        ["grouptype"] = "Group type",
        ["localauthority"] = "Local Authority",
        ["nursery"] = "Nursery provision",
        ["religiouscharacter"] = "Religious character",
        ["religiousethos"] = "Religious ethos",
        ["providertypename"] = "Provider type",
        ["sen"] = "Type of SEN provision",
        ["section41"] = "Section 41 approved (independent SEND school)",
        ["sixthform"] = "Sixth form provision",
        ["specialclass"] = "Special classes",
        ["status"] = "Status"
    };

    public IReadOnlyCollection<IFilterViewModel> Map(FacetResultsMappingContext input)
    {
        ArgumentNullException.ThrowIfNull(input);

        List<IFilterViewModel> filters = new(input.Facets.Count);

        foreach (SearchFacet searchFacet in input.Facets)
        {
            ArgumentNullException.ThrowIfNull(searchFacet);

            filters.Add(
                MapFacet(
                    searchFacet,
                    input.SelectedFacets));
        }

        return filters;
    }

    private static IFilterViewModel MapFacet(
        SearchFacet searchFacet,
        Dictionary<string, List<string>>? selectedFacets)
    {
        ArgumentNullException.ThrowIfNull(searchFacet.Name);

        List<FacetValueViewModel> values =
            MapFacetValues(
                searchFacet,
                selectedFacets);

        string label =
            _labelMappings.GetValueOrDefault(
                searchFacet.Name,
                searchFacet.Name);

        List<string> autocompleteFacets = ["localAuthority", "diocese"];

        if (autocompleteFacets.Contains(searchFacet.Name))
        {
            return new AutocompleteFilterViewModel(searchFacet.Name, label, values);
        }

        return new CheckboxFilterViewModel(
            searchFacet.Name,
            label,
            values);
    }

    private static List<FacetValueViewModel> MapFacetValues(
        SearchFacet searchFacet,
        Dictionary<string, List<string>>? selectedFacets)
    {
        ArgumentNullException.ThrowIfNull(searchFacet.Results);

        List<string> selectedValues =
            selectedFacets?.GetValueOrDefault(searchFacet.Name) ?? [];

        List<FacetValueViewModel> values = new(searchFacet.Results.Count);

        foreach (FacetResult result in searchFacet.Results)
        {
            ArgumentNullException.ThrowIfNull(result);

            values.Add(
                MapFacetValue(
                    result,
                    selectedValues));
        }

        return values;
    }

    private static FacetValueViewModel MapFacetValue(
        FacetResult result,
        IReadOnlyCollection<string> selectedValues)
    {
        ArgumentNullException.ThrowIfNull(result.Value);
        ArgumentNullException.ThrowIfNull(result.Label);

        if (result.Count is null)
        {
            throw new ArgumentException(
                "Facet result count cannot be null.",
                nameof(result));
        }

        bool isSelected =
            selectedValues.Any((selectedValue) =>
                !string.IsNullOrWhiteSpace(selectedValue) &&
                    string.Equals(
                        selectedValue,
                        result.Value,
                        StringComparison.OrdinalIgnoreCase));

        return new FacetValueViewModel(
            Value: result.Value,
            Label: result.Label,
            Count: result.Count.Value,
            IsSelected: isSelected);
    }
}
