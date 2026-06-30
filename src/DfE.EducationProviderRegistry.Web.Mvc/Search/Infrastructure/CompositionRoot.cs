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
        // Root IQueryable<Establishment>
        // ---------------------------------------------------------
        services.TryAddScoped<IQueryable<Establishment>>(sp =>
        {
            var factory = sp.GetRequiredService<IDbContextFactory<EducationProviderRegistryDbContext>>();
            var db = factory.CreateDbContext();

            return db.Establishment.AsNoTracking();
        });

        // ---------------------------------------------------------
        // Providers
        // ---------------------------------------------------------
        services.TryAddScoped<ISearchProvider<Establishment>>(sp =>
            new EstablishmentsSearchProvider(
                sp.GetRequiredService<IDbContextFactory<EducationProviderRegistryDbContext>>(),
                sp.GetRequiredService<ISearchFilterExpressionsBuilder>(),
                schemaName: "core",
                tableName: "establishment", // TODO: This lot needs to come from config, but for now we can hardcode it for now.
                searchColumn: "name"
            ));

        services.TryAddScoped<IFacetProvider<Establishment>, EstablishmentFacetProvider>();

        // ---------------------------------------------------------
        // Pipeline steps
        // ---------------------------------------------------------
        services.AddScoped<ISearchPipelineStep, SearchOrderMapStep>();
        services.AddScoped<ISearchPipelineStep, SearchOrderingStep>();
        services.AddScoped<ISearchPipelineStep>(sp =>
            new ParallelMappingStep(
                sp.GetRequiredService<IMapper<SearchResultProjection, EstablishmentSearchResult>>()));

        services.AddScoped<ISearchPipelineStep>(sp =>
            new FacetQueryDispatchStep(
                sp.GetRequiredService<IFacetProvider<Establishment>>()));

        services.AddScoped<ISearchPipelineStep, FacetQueryResolverStep>();
        services.AddScoped<ISearchPipelineStep, FacetQueryBuilderStep>();

        // ---------------------------------------------------------
        // Mappers
        // ---------------------------------------------------------
        services.TryAddSingleton<
            IMapper<SearchResultProjection, EstablishmentSearchResult>,
            EstablishmentToSearchResultMapper>();

        services.TryAddSingleton<
            IMapper<SearchPipelineContext, SearchResults<EstablishmentSearchResults, SearchFacets>>,
            SearchResultsFromContextMapper>();

        return services;
    }

    /// <summary>
    /// Extension method which provides all the pre-registrations required to
    /// access azure search filter services, and perform filtered searches across provisioned indexes.
    /// </summary>
    /// <param name="services">
    /// The originating application services onto which to register the search dependencies.
    /// </param>
    /// <param name="configuration">
    /// The originating configuration block from which to derive search service settings.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// The exception thrown if no valid T:Microsoft.Extensions.DependencyInjection.IServiceCollection
    /// is provisioned.
    /// </exception>
    public static void AddInfraSearchFilterDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services),
                "A service collection is required to configure the azure cognitive search filter dependencies.");
        }

        services.TryAddScoped<IFilterExpressionFormatter, DefaultFilterExpressionFormatter>();
        services.TryAddScoped<AndLogicalOperator>();
        services.TryAddScoped<OrLogicalOperator>();
        services.TryAddScoped<SingleOrMultiValueEqualsExpression>();
        services.TryAddScoped<ISearchFilterExpressionsBuilder, SearchFilterExpressionsBuilder>();
        services.TryAddSingleton<ISearchFilterExpressionFactory>(provider =>
        {
            var scopedSearchFilterExpressionProvider = provider.CreateScope();
            var searchFilterExpressions =
                new Dictionary<string, Func<ISearchFilterExpression>>()
                {
                    ["SingleOrMultiValueEqualsExpression"] = () =>
                        scopedSearchFilterExpressionProvider
                            .ServiceProvider.GetRequiredService<SingleOrMultiValueEqualsExpression>()
                };

            return new SearchFilterExpressionFactory(searchFilterExpressions);
        });

        // ---------------------------------------------------------
        // Mappers
        // ---------------------------------------------------------
        services.TryAddSingleton<IMapper<
            ReadOnlyCollection<FilterRequest>,
            ReadOnlyCollection<SearchFilterRequest>>, SearchRequestFiltersToCoreFiltersMapper>();

        // ---------------------------------------------------------
        // Facet selectors - we could do more with this going 
        // forward (build this dynamically via config), but for now
        // we just need to be able to map the facet name to the
        // property on the Establishment entity.
        // ---------------------------------------------------------
        services.AddSingleton<
            Dictionary<string, Expression<Func<Establishment, object>>>>
            (
                new Dictionary<string, Expression<Func<Establishment, object>>>(StringComparer.OrdinalIgnoreCase)
                {
                    { "establishmenttypeid", establishment => establishment.EstablishmentTypeId }
                }
            );

        services.TryAddSingleton<ILogicalOperatorFactory>(provider =>
        {
            var scopedLogicalOperatorExpressionProvider = provider.CreateScope();
            var logicalOperators =
                new Dictionary<string, Func<ILogicalOperator>>()
                {
                    ["AndLogicalOperator"] = () =>
                           scopedLogicalOperatorExpressionProvider
                               .ServiceProvider.GetRequiredService<AndLogicalOperator>(),
                    ["OrLogicalOperator"] = () =>
                        scopedLogicalOperatorExpressionProvider
                            .ServiceProvider.GetRequiredService<OrLogicalOperator>()
                };

            return new LogicalOperatorFactory(logicalOperators);
        });

        services.AddOptions<FilterKeyToFilterExpressionMapOptions>()
            .Configure<IConfiguration>(
                (settings, configuration) =>
                    configuration
                        .GetSection("FilterKeyToFilterExpressionMapOptions")
                        .Bind(settings))
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
