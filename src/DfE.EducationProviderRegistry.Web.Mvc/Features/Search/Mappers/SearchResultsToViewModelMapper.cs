using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;

public class SearchResultsToViewModelMapper :
    IMapper<UseCaseResponse<SearchResponse>, SearchResultsViewModel>
{
    private readonly IMapper<IReadOnlyCollection<EstablishmentSearchResult>, List<GovUkTable>> _establishmentSearchResultsToViewModelMapper;
    private readonly IMapper<IReadOnlyCollection<SearchFacet>, List<FacetViewModel>> _facetResultsToFacetsViewModelMapper;

    public SearchResultsToViewModelMapper(
        IMapper<IReadOnlyCollection<EstablishmentSearchResult>, List<GovUkTable>> establishmentSearchResultsToViewModelMapper,
        IMapper<IReadOnlyCollection<SearchFacet>, List<FacetViewModel>> facetResultsToFacetsViewModelMapper)
    {
        _establishmentSearchResultsToViewModelMapper = establishmentSearchResultsToViewModelMapper;
        _facetResultsToFacetsViewModelMapper = facetResultsToFacetsViewModelMapper;
    }

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
