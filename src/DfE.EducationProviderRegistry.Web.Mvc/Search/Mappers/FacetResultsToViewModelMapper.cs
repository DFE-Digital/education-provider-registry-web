using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Web.Mvc.Search.ViewModels;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Mappers;

public sealed class FacetResultsToViewModelMapper :
    IMapper<IReadOnlyCollection<SearchFacet>, List<FacetViewModel>>
{
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

        if (result.Count == null)
        {
            throw new ArgumentException(
                "Facet result count cannot be null.",
                nameof(result)
            );
        }

        return new FacetValueViewModel(
            Value: result.Value,
            Count: result.Count.Value,
            IsSelected: true
        );
    }
}
