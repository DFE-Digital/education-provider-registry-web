using System.Collections.ObjectModel;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Services;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.Request;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.Results;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search;

public sealed class SearchServiceCollectionExtensionsTests
{
    [Fact]
    public void AddSearch_GivenNullServices_ThrowsArgumentNullException()
    {
        // Arrange
        IConfiguration configuration =
            CreateConfiguration();

        // Act
        Func<IServiceCollection> addSearch = () =>
            SearchServiceCollectionExtensions.AddSearch(
                null!,
                configuration);

        // Assert
        Assert.Throws<ArgumentNullException>(addSearch);
    }

    [Fact]
    public void AddSearch_GivenNullConfiguration_ThrowsArgumentNullException()
    {
        // Arrange
        IServiceCollection services =
            new ServiceCollection();

        // Act
        Func<IServiceCollection> addSearch = () =>
            services.AddSearch(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(addSearch);
    }

    [Fact]
    public void AddSearch_ReturnsServices()
    {
        // Arrange
        IServiceCollection services =
            new ServiceCollection();

        IConfiguration configuration =
            CreateConfiguration();

        // Act
        IServiceCollection result =
            services.AddSearch(configuration);

        // Assert
        Assert.Same(
            services,
            result);
    }

    [Fact]
    public void AddSearch_RegistersSearchResultsToViewModelMapperAsSingleton()
    {
        // Arrange
        IServiceCollection services =
            new ServiceCollection();

        IConfiguration configuration =
            CreateConfiguration();

        // Act
        services.AddSearch(configuration);

        // Assert
        AssertSingletonRegistration<
            IMapper<
                SearchResultsMappingContext,
                SearchResultsViewModel>,
            SearchResultsToViewModelMapper>(
                services);
    }

    [Fact]
    public void AddSearch_RegistersFacetResultsToViewModelMapperAsSingleton()
    {
        // Arrange
        IServiceCollection services =
            new ServiceCollection();

        IConfiguration configuration =
            CreateConfiguration();

        // Act
        services.AddSearch(configuration);

        // Assert
        AssertSingletonRegistration<
            IMapper<
                FacetResultsMappingContext,
                IReadOnlyCollection<IFilterViewModel>>,
            FacetResultsToViewModelMapper>(
                services);
    }

    [Fact]
    public void AddSearch_RegistersAvailableFiltersToViewModelMapperAsSingleton()
    {
        // Arrange
        IServiceCollection services =
            new ServiceCollection();

        IConfiguration configuration =
            CreateConfiguration();

        // Act
        services.AddSearch(configuration);

        // Assert
        AssertSingletonRegistration<
            IMapper<
                AvailableFiltersMappingContext,
                IReadOnlyCollection<IFilterViewModel>>,
            AvailableFiltersToViewModelMapper>(
                services);
    }

    [Fact]
    public void AddSearch_RegistersSearchAggregateResultsToViewModelMapperAsSingleton()
    {
        // Arrange
        IServiceCollection services =
            new ServiceCollection();

        IConfiguration configuration =
            CreateConfiguration();

        // Act
        services.AddSearch(configuration);

        // Assert
        AssertSingletonRegistration<
            IMapper<
                IReadOnlyCollection<SearchAggregateResult>,
                List<GovUkTable>>,
            SearchAggregateResultsToViewModelMapper>(
                services);
    }

    [Fact]
    public void AddSearch_RegistersSelectedFacetsToFilterRequestsMapperAsSingleton()
    {
        // Arrange
        IServiceCollection services =
            new ServiceCollection();

        IConfiguration configuration =
            CreateConfiguration();

        // Act
        services.AddSearch(configuration);

        // Assert
        AssertSingletonRegistration<
            IMapper<
                Dictionary<string, List<string>>?,
                ReadOnlyCollection<FilterRequest>>,
            SelectedFacetsToFilterRequestsMapper>(
                services);
    }

    [Fact]
    public void AddSearch_RegistersSearchFiltersToFilterRequestsMapperAsSingleton()
    {
        // Arrange
        IServiceCollection services =
            new ServiceCollection();

        IConfiguration configuration =
            CreateConfiguration();

        // Act
        services.AddSearch(configuration);

        // Assert
        AssertSingletonRegistration<
            IMapper<
                SearchRequestViewModel,
                ReadOnlyCollection<FilterRequest>>,
            SearchFiltersToFilterRequestsMapper>(
                services);
    }

    [Fact]
    public void AddSearch_RegistersSearchFilterSelectionHandlerAsSingleton()
    {
        // Arrange
        IServiceCollection services =
            new ServiceCollection();

        IConfiguration configuration =
            CreateConfiguration();

        // Act
        services.AddSearch(configuration);

        // Assert
        AssertSingletonRegistration<
            ISearchFilterSelectionHandler,
            SearchFilterSelectionHandler>(
                services);
    }

    [Fact]
    public void AddSearch_RegistersEstablishmentCategoryMapperAsSingleton()
    {
        // Arrange
        IServiceCollection services =
            new ServiceCollection();

        IConfiguration configuration =
            CreateConfiguration();

        // Act
        services.AddSearch(configuration);

        // Assert
        AssertSingletonRegistration<
            ISearchAggregateCategoryToModelMapper,
            SearchAggregateCategoryEstablishmentMapper>(
                services);
    }

    [Fact]
    public void AddSearch_RegistersGroupCategoryMapperAsSingleton()
    {
        // Arrange
        IServiceCollection services =
            new ServiceCollection();

        IConfiguration configuration =
            CreateConfiguration();

        // Act
        services.AddSearch(configuration);

        // Assert
        AssertSingletonRegistration<
            ISearchAggregateCategoryToModelMapper,
            SearchAggregateCategoryGroupMapper>(
                services);
    }

    private static IConfiguration CreateConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>()
                {
                    { "SearchConfiguration" , "" }
                })
            .Build();
    }

    private static void AssertSingletonRegistration<TService, TImplementation>(
        IServiceCollection services)
    {
        ServiceDescriptor registration =
            Assert.Single(
                services,
                descriptor =>
                    descriptor.ServiceType == typeof(TService) &&
                    descriptor.ImplementationType == typeof(TImplementation));

        Assert.Equal(
            ServiceLifetime.Singleton,
            registration.Lifetime);
    }
}
