using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;

/// <summary>
/// Maps a <see cref="UseCaseResponse{TModel}"/> containing a <see cref="SearchResponse"/>
/// into a <see cref="SearchResultsViewModel"/> suitable for rendering in the MVC UI.
/// </summary>
/// <remarks>
/// This mapper coordinates two specialised mappers:
/// <list type="bullet">
/// <item>
/// <description>
/// <see cref="IMapper{TInput, TOutput}"/> for converting establishment search results
/// into <see cref="GovUkTable"/> components.
/// </description>
/// </item>
/// <item>
/// <description>
/// <see cref="IMapper{TInput, TOutput}"/> for converting facet results into
/// <see cref="FacetViewModel"/> collections.
/// </description>
/// </item>
/// </list>
/// 
/// It performs no business logic, filtering, or sorting. Its sole responsibility is
/// transforming the domain-level <see cref="SearchResponse"/> into a UI-ready
/// <see cref="SearchResultsViewModel"/>.
/// </remarks>
public class SearchResultsToViewModelMapper :
    IMapper<UseCaseResponse<SearchResponse>, SearchResultsViewModel>
{
    private readonly IMapper<IReadOnlyCollection<EstablishmentSearchResult>, List<GovUkTable>> _establishmentSearchResultsToViewModelMapper;
    private readonly IMapper<IReadOnlyCollection<SearchFacet>, List<FacetViewModel>> _facetResultsToFacetsViewModelMapper;

    /// <summary>
    /// Creates a new instance of <see cref="SearchResultsToViewModelMapper"/>.
    /// </summary>
    /// <param name="establishmentSearchResultsToViewModelMapper">
    /// Mapper responsible for converting establishment search results into
    /// <see cref="GovUkTable"/> view components.
    /// </param>
    /// <param name="facetResultsToFacetsViewModelMapper">
    /// Mapper responsible for converting facet results into <see cref="FacetViewModel"/> instances.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when any dependency is <c>null</c>.
    /// </exception>
    public SearchResultsToViewModelMapper(
        IMapper<IReadOnlyCollection<EstablishmentSearchResult>, List<GovUkTable>> establishmentSearchResultsToViewModelMapper,
        IMapper<IReadOnlyCollection<SearchFacet>, List<FacetViewModel>> facetResultsToFacetsViewModelMapper)
    {
        _establishmentSearchResultsToViewModelMapper = establishmentSearchResultsToViewModelMapper;
        _facetResultsToFacetsViewModelMapper = facetResultsToFacetsViewModelMapper;
    }

    /// <summary>
    /// Maps a <see cref="UseCaseResponse{SearchResponse}"/> into a <see cref="SearchResultsViewModel"/>.
    /// </summary>
    /// <param name="input">The use case response containing the search results.</param>
    /// <returns>
    /// A populated <see cref="SearchResultsViewModel"/> containing establishment tables
    /// and facet view models.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="input"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <see cref="UseCaseResponse{TModel}.Model"/> is <c>null</c>.
    /// </exception>
    public SearchResultsViewModel Map(UseCaseResponse<SearchResponse> input)
    {
        ArgumentNullException.ThrowIfNull(input);

        if (input.Model == null)
        {
            throw new ArgumentException("SearchResponse model cannot be null.", nameof(input));
        }

        return new()
        {
            EstablishmentResults =
                (input.Model.EstablishmentResults != null)
                    ? _establishmentSearchResultsToViewModelMapper.Map(
                        input.Model.EstablishmentResults.EstablishmentCollection)
                    : [],

            Facets =
                (input.Model.FacetedResults != null)
                    ? _facetResultsToFacetsViewModelMapper.Map(
                        input.Model.FacetedResults.Facets)
                    : []
        };
    }
}
