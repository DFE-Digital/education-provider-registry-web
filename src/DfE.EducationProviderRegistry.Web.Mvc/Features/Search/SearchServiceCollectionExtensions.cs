using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;
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
                UseCaseResponse<SearchResponse>, SearchResultsViewModel>, SearchResultsToViewModelMapper>()
            .AddSingleton<IMapper<
                IReadOnlyCollection<SearchFacet>, List<FacetViewModel>>, FacetResultsToViewModelMapper>()
            .AddSingleton<IMapper<
                IReadOnlyCollection<EstablishmentSearchResult>, List<GovUkTable>>, EstablishmentSearchResultsToViewModelMapper>()
            .AddSingleton<IMapper<
                Dictionary<string, List<string>>?, ReadOnlyCollection<FilterRequest>>, SelectedFacetsToFilterRequestsMapper>();

        return services;
    }
}
