using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;

/// <summary>
/// Maps search facet results produced by the search engine into
/// <see cref="FacetViewModel"/> instances suitable for rendering in the UI.
/// </summary>
/// <remarks>
/// This mapper performs a direct transformation from domain-level facet
/// structures (<see cref="SearchFacet"/> and <see cref="FacetResult"/>) into
/// MVC view models. It does not apply filtering, sorting, or selection logic.
/// 
/// Each <see cref="SearchFacet"/> becomes a <see cref="FacetViewModel"/>,
/// and each <see cref="FacetResult"/> becomes a <see cref="FacetValueViewModel"/>.
/// </remarks>
public sealed class FacetResultsToViewModelMapper :
    IMapper<IReadOnlyCollection<SearchFacet>, List<FacetViewModel>>
{
    /// <summary>
    /// Maps a collection of <see cref="SearchFacet"/> instances into a list of
    /// <see cref="FacetViewModel"/> objects.
    /// </summary>
    /// <param name="input">The facet results returned by the search engine.</param>
    /// <returns>
    /// A list of <see cref="FacetViewModel"/> representing the facet categories
    /// and their available values.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="input"/> is <c>null</c>.
    /// </exception>
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

    /// <summary>
    /// Maps a single <see cref="SearchFacet"/> into a <see cref="FacetViewModel"/>.
    /// </summary>
    /// <param name="searchFacet">The facet to map.</param>
    /// <returns>A populated <see cref="FacetViewModel"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="searchFacet"/> or its <see cref="SearchFacet.Name"/> is <c>null</c>.
    /// </exception>
    private static FacetViewModel MapFacet(SearchFacet searchFacet)
    {
        ArgumentNullException.ThrowIfNull(searchFacet.Name);

        List<FacetValueViewModel> values = MapFacetValues(searchFacet);

        return new FacetViewModel(
            Name: searchFacet.Name,
            Values: values
        );
    }

    /// <summary>
    /// Maps the collection of <see cref="FacetResult"/> values belonging to a facet.
    /// </summary>
    /// <param name="searchFacet">The facet whose values are being mapped.</param>
    /// <returns>
    /// A list of <see cref="FacetValueViewModel"/> representing the facet values.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="searchFacet"/> or its <see cref="SearchFacet.Results"/> is <c>null</c>.
    /// </exception>
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

    /// <summary>
    /// Maps a single <see cref="FacetResult"/> into a <see cref="FacetValueViewModel"/>.
    /// </summary>
    /// <param name="result">The facet value to map.</param>
    /// <returns>
    /// A <see cref="FacetValueViewModel"/> containing the value, count, and selection state.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="result"/> or its <see cref="FacetResult.Value"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <see cref="FacetResult.Count"/> is <c>null</c>.
    /// </exception>
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