using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;

public class SearchResultsToViewModelMapper : IMapper<EstablishmentSearchResults, SearchResultsViewModel>
{
    private readonly IMapper<EstablishmentSearchResult, GovUkTable> _searchResultToTableViewModelMapper;

    public SearchResultsToViewModelMapper(
        IMapper<EstablishmentSearchResult, GovUkTable> searchResultToTableViewModelMapper)
    {
        _searchResultToTableViewModelMapper = searchResultToTableViewModelMapper;
    }

    public SearchResultsViewModel Map(EstablishmentSearchResults input) =>
        new()
        {
            EstablishmentResults =
                [.. input.EstablishmentCollection.Select(
                    _searchResultToTableViewModelMapper.Map)]
        };
}