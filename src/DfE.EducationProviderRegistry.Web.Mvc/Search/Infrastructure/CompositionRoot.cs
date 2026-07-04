using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering.FilterExpressions;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering.FilterExpressions.Factories;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering.FilterExpressions.Formatters;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering.LogicalOperators;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering.Options;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers.SearchOrchestrators;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers.SearchOrchestrators.EFMetadataResolver;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.FilterExpressions;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Pipeline;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Pipeline.Steps;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Providers;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Providers.Projections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure;

public static class CompositionRoot
{
    public static IServiceCollection AddInfraSearchDependencies(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        string connectionString =
            "Host=localhost;Port=5440;Database=education-provider-registr-prototype-baseline;Username=postgres;Password=postgres";

        // ---------------------------------------------------------
        // DbContext factory 
        // ---------------------------------------------------------
        services.AddDbContextFactory<EducationProviderRegistryDbContext>(options =>
            options.UseNpgsql(connectionString)
                   .EnableSensitiveDataLogging()
                   .EnableDetailedErrors()
                   .LogTo(Console.WriteLine, LogLevel.Information));

        // ---------------------------------------------------------
        // Search orchestrator (trigram)
        // ---------------------------------------------------------
        services.TryAddScoped<ISearchOrchestrator<Establishment>, TrigramSearchOrchestrator<Establishment>>();

        // ---------------------------------------------------------
        // Projection builder
        // ---------------------------------------------------------
        services.TryAddScoped<ISearchProjectionBuilder<Establishment>,
            EstablishmentSearchProjectionBuilder>();

        // ---------------------------------------------------------
        // Search orchestration
        // ---------------------------------------------------------
        services.AddSingleton(typeof(IEntityMetadataResolver<>), typeof(CachedEntityMetadataResolver<>));
        services.AddScoped(typeof(ISearchOrchestrator<>), typeof(TrigramSearchOrchestrator<>));

        // ---------------------------------------------------------
        // SQL executor (required for trigram orchestrator)
        // ---------------------------------------------------------
        services.AddScoped(typeof(ISqlExecutor<>), typeof(SqlExecutor<>));

        // ---------------------------------------------------------
        // Providers
        // ---------------------------------------------------------
        services.TryAddScoped<ISearchProvider<Establishment>>(sp =>
            new EstablishmentsSearchProvider(
                sp.GetRequiredService<IDbContextFactory<EducationProviderRegistryDbContext>>(),
                sp.GetRequiredService<ISearchOrchestrator<Establishment>>(),
                sp.GetRequiredService<ISearchProjectionBuilder<Establishment>>(),
                sp.GetRequiredService<ISearchFilterExpressionsBuilder>(),
                searchColumn: "name" // TODO: move to config
            ));

        services.TryAddScoped<IFacetProvider<Establishment>, EstablishmentFacetProvider>();

        // ---------------------------------------------------------
        // Pipeline steps
        // ---------------------------------------------------------
        services.AddScoped<ISearchPipelineStep, SearchOrderMapStep>();
        services.AddScoped<ISearchPipelineStep, SearchOrderingStep>();
        services.AddScoped<ISearchPipelineStep>(sp =>
            new ParallelMappingStep(
                sp.GetRequiredService<IMapper<Establishment, EstablishmentSearchResult>>()));

        services.AddScoped<ISearchPipelineStep>(sp =>
            new FacetQueryDispatchStep(
                sp.GetRequiredService<IFacetProvider<Establishment>>()));

        services.AddScoped<ISearchPipelineStep, FacetQueryResolverStep>();
        services.AddScoped<ISearchPipelineStep, FacetQueryBuilderStep>();

        // ---------------------------------------------------------
        // Mappers
        // ---------------------------------------------------------
        services.TryAddSingleton<
            IMapper<Establishment, EstablishmentSearchResult>,
            EstablishmentToSearchResultMapper>();

        services.TryAddSingleton<
            IMapper<SearchPipelineContext, SearchResults<EstablishmentSearchResults, SearchFacets>>,
            SearchResultsFromContextMapper>();

        return services;
    }

    public static void AddInfraSearchFilterDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddScoped<IFilterExpressionFormatter, DefaultFilterExpressionFormatter>();
        services.TryAddScoped<AndLogicalOperator>();
        services.TryAddScoped<OrLogicalOperator>();
        services.TryAddScoped<SingleOrMultiValueEqualsExpression>();
        services.TryAddScoped<ISearchFilterExpressionsBuilder, SearchFilterExpressionsBuilder>();

        services.TryAddSingleton<ISearchFilterExpressionFactory>(provider =>
        {
            var scoped = provider.CreateScope();
            var map = new Dictionary<string, Func<ISearchFilterExpression>>
            {
                ["SingleOrMultiValueEqualsExpression"] = () =>
                    scoped.ServiceProvider.GetRequiredService<SingleOrMultiValueEqualsExpression>()
            };

            return new SearchFilterExpressionFactory(map);
        });

        // ---------------------------------------------------------
        // Mappers
        // ---------------------------------------------------------
        services.TryAddSingleton<IMapper<
            ReadOnlyCollection<FilterRequest>,
            ReadOnlyCollection<SearchFilterRequest>>, SearchRequestFiltersToCoreFiltersMapper>();

        // ---------------------------------------------------------
        // Facet selectors
        // ---------------------------------------------------------
        services.AddSingleton<
            Dictionary<string, Expression<Func<Establishment, object>>>>
            (
                new Dictionary<string, Expression<Func<Establishment, object>>>(StringComparer.OrdinalIgnoreCase)
                {
                    { "establishmenttypeid", e => e.EstablishmentTypeId }
                }
            );

        services.TryAddSingleton<ILogicalOperatorFactory>(provider =>
        {
            var scoped = provider.CreateScope();
            var map = new Dictionary<string, Func<ILogicalOperator>>
            {
                ["AndLogicalOperator"] = () =>
                    scoped.ServiceProvider.GetRequiredService<AndLogicalOperator>(),
                ["OrLogicalOperator"] = () =>
                    scoped.ServiceProvider.GetRequiredService<OrLogicalOperator>()
            };

            return new LogicalOperatorFactory(map);
        });

        services.AddOptions<FilterKeyToFilterExpressionMapOptions>()
            .Configure<IConfiguration>(
                (settings, cfg) =>
                    cfg.GetSection("FilterKeyToFilterExpressionMapOptions").Bind(settings))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // ---------------------------------------------------------
        // Search service adapter
        // ---------------------------------------------------------
        services.AddScoped<
            ISearchServiceAdapter<EstablishmentSearchResults, SearchFacets>,
            EstablishmentsSearchServiceAdapter>();
    }
}

