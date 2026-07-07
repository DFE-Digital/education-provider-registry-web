using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Filtering.FilterExpressions.Factories;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Filtering.FilterExpressions.Formatters;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Filtering.LogicalOperators.Factories;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Providers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Providers.SearchOrchestrators;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Pipeline;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Pipeline.Steps;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Providers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Providers.Projections;
using DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Infrastructure.TestDoubles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Infrastructure;

public sealed class CompositionRootUnitTests
{
    [Fact]
    public void CompositionRoot_Registers_DbContextFactory()
    {
        // arrange
        IServiceProvider provider =
            ServiceProviderBuilder.BuildServiceProvider();

        // act
        IDbContextFactory<EducationProviderRegistryDbContext> factory =
            provider.GetRequiredService<
                IDbContextFactory<EducationProviderRegistryDbContext>>();

        // assert
        Assert.NotNull(factory);
    }

    [Fact]
    public void CompositionRoot_Registers_FilterExpressionFactory()
    {
        // arrange
        IServiceProvider provider =
            ServiceProviderBuilder.BuildServiceProvider();

        // act
        ISearchFilterExpressionFactory factory =
            provider.GetRequiredService<
                ISearchFilterExpressionFactory>();

        // assert
        Assert.NotNull(factory);
    }

    [Fact]
    public void CompositionRoot_Registers_LogicalOperatorFactory()
    {
        // arrange
        IServiceProvider provider =
            ServiceProviderBuilder.BuildServiceProvider();

        // act
        ILogicalOperatorFactory factory =
            provider.GetRequiredService<
                ILogicalOperatorFactory>();

        // assert
        Assert.NotNull(factory);
    }

    [Fact]
    public void CompositionRoot_Registers_FacetSelectorDictionary()
    {
        // arrange
        IServiceProvider provider =
            ServiceProviderBuilder.BuildServiceProvider();

        // act
        Dictionary<string, Expression<Func<Establishment, object>>> selectors =
            provider.GetRequiredService<
                Dictionary<string, Expression<Func<Establishment, object>>>>();

        // assert
        Assert.True(selectors.ContainsKey("establishmenttypeid"));
    }

    [Fact]
    public void CompositionRoot_Registers_FilterExpressionFormatter()
    {
        // arrange
        IServiceProvider provider =
            ServiceProviderBuilder.BuildServiceProvider();

        using IServiceScope scope = provider.CreateScope();

        // act
        IFilterExpressionFormatter formatter =
            scope.ServiceProvider.GetRequiredService<
                IFilterExpressionFormatter>();

        // assert
        Assert.IsType<DefaultFilterExpressionFormatter>(formatter);
    }

    [Fact]
    public void CompositionRoot_Registers_SearchOrchestrator()
    {
        // arrange
        IServiceProvider provider =
            ServiceProviderBuilder.BuildServiceProvider();

        using IServiceScope scope = provider.CreateScope();

        // act
        ISearchOrchestrator<Establishment> orchestrator =
            scope.ServiceProvider.GetRequiredService<
                ISearchOrchestrator<Establishment>>();

        // assert
        Assert.IsType<TrigramSearchOrchestrator<Establishment>>(orchestrator);
    }

    [Fact]
    public void CompositionRoot_Registers_ProjectionBuilder()
    {
        // arrange
        IServiceProvider provider =
            ServiceProviderBuilder.BuildServiceProvider();

        using IServiceScope scope = provider.CreateScope();

        // act
        ISearchProjectionBuilder<Establishment> builder =
            scope.ServiceProvider.GetRequiredService<
                ISearchProjectionBuilder<Establishment>>();

        // assert
        Assert.IsType<EstablishmentSearchProjectionBuilder>(builder);
    }

    [Fact]
    public void CompositionRoot_Registers_SearchProvider()
    {
        // arrange
        IServiceProvider provider =
            ServiceProviderBuilder.BuildServiceProvider();

        using IServiceScope scope = provider.CreateScope();

        // act
        ISearchProvider<Establishment> providerInstance =
            scope.ServiceProvider.GetRequiredService<
                ISearchProvider<Establishment>>();

        // assert
        Assert.IsType<EstablishmentsSearchProvider>(providerInstance);
    }

    [Fact]
    public void CompositionRoot_Registers_FacetProvider()
    {
        // arrange
        IServiceProvider provider =
            ServiceProviderBuilder.BuildServiceProvider();

        using IServiceScope scope = provider.CreateScope();

        // act
        IFacetProvider facetProvider =
            scope.ServiceProvider.GetRequiredService<IFacetProvider>();

        // assert
        Assert.IsType<EstablishmentFacetProvider>(facetProvider);
    }

    [Fact]
    public void CompositionRoot_Registers_PipelineSteps()
    {
        // arrange
        IServiceProvider provider =
            ServiceProviderBuilder.BuildServiceProvider();

        using IServiceScope scope = provider.CreateScope();

        // act
        IEnumerable<ISearchPipelineStep> steps =
            scope.ServiceProvider.GetServices<ISearchPipelineStep>();

        // assert
        Assert.Contains(steps, step => step is SearchOrderMapStep);
        Assert.Contains(steps, step => step is SearchOrderingStep);
        Assert.Contains(steps, step => step is ParallelMappingStep);
        Assert.Contains(steps, step => step is FacetQueryDispatchStep);
        Assert.Contains(steps, step => step is FacetQueryResolverStep);
        Assert.Contains(steps, step => step is FacetQueryBuilderStep);
    }

    [Fact]
    public void CompositionRoot_Registers_SearchServiceAdapter()
    {
        // arrange
        IServiceProvider provider =
            ServiceProviderBuilder.BuildServiceProvider();

        using IServiceScope scope = provider.CreateScope();

        // act
        ISearchServiceAdapter<EstablishmentSearchResults, SearchFacets> adapter =
            scope.ServiceProvider.GetRequiredService<
                ISearchServiceAdapter<EstablishmentSearchResults, SearchFacets>>();

        // assert
        Assert.IsType<EstablishmentsSearchServiceAdapter>(adapter);
    }
}
