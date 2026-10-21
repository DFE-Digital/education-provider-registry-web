using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Resolvers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Store;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies.Factories;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies.JourneyRules;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Tracking;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs;

public static class CompositionRoot
{
    public static IServiceCollection AddBreadcrumbDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        // Configuration Options.
        services.Configure<BreadcrumbOptions>(configuration.GetSection("Breadcrumbs"));

        // Resolvers (Routing and Title Resolution).
        services.AddSingleton<IBreadcrumbRouteResolver, BreadcrumbRouteResolver>();
        services.AddSingleton<IBreadcrumbTitleResolver, BreadcrumbTitleResolver>();

        // State Store & Evaluation Infrastructure.
        services.AddScoped<IBreadcrumbJourneyStore, SessionBreadcrumbJourneyStore>();

        // Incremental Journey Rules (Evaluated in order).
        services.AddTransient<IJourneyRule, EmptyJourneyRule>();
        services.AddTransient<IJourneyRule, ExactUrlMatchRule>();
        services.AddTransient<IJourneyRule, LastPathMatchRule>();
        services.AddTransient<IJourneyRule, ExistingPathMatchRule>();
        services.AddTransient<IJourneyRule, DefaultFallbackRule>();

        // Strategies and Strategy Factory.
        services.AddKeyedScoped<IBreadcrumbStrategy, BaseJourneyStrategy>("BaseJourney");
        services.AddKeyedScoped<IBreadcrumbStrategy, IncrementalJourneyStrategy>("IncrementalJourney");
        services.AddScoped<IBreadcrumbStrategyFactory, BreadcrumbStrategyFactory>();

        // Tracking & MVC Filter Pipeline.
        services.AddTransient<IBreadcrumbTracker, BreadcrumbTracker>();
        services.AddScoped<BreadcrumbResultFilter>();

        return services;
    }
}
