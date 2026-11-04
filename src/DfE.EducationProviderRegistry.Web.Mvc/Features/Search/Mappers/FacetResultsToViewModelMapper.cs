using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;

public sealed class FacetResultsToViewModelMapper :
    IMapper<IReadOnlyCollection<SearchFacet>, List<FacetViewModel>>
{
    private static readonly Dictionary<string, string> _labelMappings = new()
    {
        ["admissionpolicy"] = "Admission policy",
        ["boarding"] = "Boarding provision",
        ["diocese"] = "Diocese",
        ["educationphase"] = "Phase of education",
        ["furthereducationtype"] = "Further education type",
        ["gender"] = "Gender",
        ["grouptype"] = "Group type",
        ["LocalAuthority"] = "Local Authority",
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

    public List<FacetViewModel> Map(IReadOnlyCollection<SearchFacet> input)
    {
        ArgumentNullException.ThrowIfNull(input);

        List<FacetViewModel> facets = new(input.Count);

        foreach (SearchFacet searchFacet in input)
        {
            ArgumentNullException.ThrowIfNull(searchFacet);

            FacetViewModel facetViewModel = MapFacet(searchFacet);
            facets.Add(facetViewModel);
        }

        return facets;
    }

    private static FacetViewModel MapFacet(SearchFacet searchFacet)
    {
        ArgumentNullException.ThrowIfNull(searchFacet.Name);

        List<FacetValueViewModel> values = MapFacetValues(searchFacet);

        return new FacetViewModel(
            Name: searchFacet.Name,
            Label: _labelMappings.GetValueOrDefault(searchFacet.Name, searchFacet.Name),
            Values: values
        );
    }

    private static List<FacetValueViewModel> MapFacetValues(SearchFacet searchFacet)
    {
        ArgumentNullException.ThrowIfNull(searchFacet.Results);

        List<FacetValueViewModel> values = new(searchFacet.Results.Count);

        foreach (FacetResult result in searchFacet.Results)
        {
            ArgumentNullException.ThrowIfNull(result);

            FacetValueViewModel valueViewModel = MapFacetValue(result);
            values.Add(valueViewModel);
        }

        return values;
    }

    private static FacetValueViewModel MapFacetValue(FacetResult result)
    {
        ArgumentNullException.ThrowIfNull(result.Value);
        ArgumentNullException.ThrowIfNull(result.Label);

        if (result.Count == null)
        {
            throw new ArgumentException(
                "Facet result count cannot be null.",
                nameof(result)
            );
        }

        return new FacetValueViewModel(
            Value: result.Value,
            Label: result.Label,
            Count: result.Count.Value,
            IsSelected: false
        );
    }
}
