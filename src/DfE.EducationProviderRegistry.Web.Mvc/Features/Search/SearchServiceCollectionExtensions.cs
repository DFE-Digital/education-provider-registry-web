using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Services;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;
using DfE.EducationProviderRegistry.Web.Mvc.ViewModels;
using System.Collections.ObjectModel;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search;

internal static class SearchServiceCollectionExtensions
{
    internal static IServiceCollection AddSearch(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        // Core
        services
            .AddApplicationSearchDependencies(configuration)
            .AddInfraSearchDependencies(configuration)
            .AddInfraSearchFilterDependencies(configuration);

        // Presentation
        services
            .AddSingleton<IMapper<
                SearchResultsMappingContext, SearchResultsViewModel>, SearchResultsToViewModelMapper>()
            .AddSingleton<IMapper<
                IReadOnlyCollection<SearchFacet>, List<FacetViewModel>>, FacetResultsToViewModelMapper>()
            .AddSingleton<IMapper<
                IReadOnlyCollection<EstablishmentSearchResult>, List<GovUkTable>>, EstablishmentSearchResultsToViewModelMapper>()
            .AddSingleton<IMapper<
                Dictionary<string, List<string>>?, ReadOnlyCollection<FilterRequest>>, SelectedFacetsToFilterRequestsMapper>()
            .AddSingleton<IMapper<
                SearchFiltersMappingContext, SearchFiltersViewModel>, SearchResponseToSearchFiltersViewModelMapper>()
            .AddSingleton<ISearchFilterSelectionHandler, SearchFilterSelectionHandler>();

        return services;
    }
}
