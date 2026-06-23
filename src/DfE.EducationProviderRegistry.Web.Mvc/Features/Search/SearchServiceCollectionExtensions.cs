using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;
using Microsoft.Extensions.Options;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search;

internal static class SearchServiceCollectionExtensions
{
    internal static IServiceCollection AddSearch(this IServiceCollection services)
    {
        services
            .AddSingleton<IMapper<EstablishmentSearchResults, SearchResultsViewModel>, SearchResultsToViewModelMapper>()
            .AddSingleton<IMapper<EstablishmentSearchResult, GovUkTable>, SearchResultToTableViewModelMapper>();

        services.AddSearchDependencies();

        // Bind search criteria configuration options.
        services.AddOptions<SearchCriteria>()
            .Configure<IConfiguration>((settings, configuration) =>
                configuration
                    .GetSection(nameof(SearchCriteria))
                    .Bind(settings));

        // Register strongly typed configuration instances.
        services.AddSingleton((serviceProvider) =>
            serviceProvider.GetRequiredService<IOptions<SearchCriteria>>().Value);

        return services;
    }
}
