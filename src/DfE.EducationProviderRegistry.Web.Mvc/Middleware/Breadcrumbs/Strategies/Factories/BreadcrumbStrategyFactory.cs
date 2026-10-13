using DfE.EducationProviderRegistry.Web.Mvc.Middleware.Breadcrumbs.Options;

namespace DfE.EducationProviderRegistry.Web.Mvc.Middleware.Breadcrumbs.Strategies.Factories;

public sealed class BreadcrumbStrategyFactory : IBreadcrumbStrategyFactory
{
    private readonly IServiceProvider _serviceProvider;

    public BreadcrumbStrategyFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ??
            throw new ArgumentNullException(nameof(serviceProvider));
    }

    public IBreadcrumbStrategy GetStrategy(BreadcrumbRouteRule rule)
    {
        const string BaseJourneyStrategyKey = "BaseJourney";
        const string IncrementalJourneyStrategyKey = "IncrementalJourney";

        string strategyKey =
            rule.BaseJourney.Count > 0 ?
                BaseJourneyStrategyKey :
                IncrementalJourneyStrategyKey;

        IBreadcrumbStrategy? strategy =
            _serviceProvider.GetKeyedService<IBreadcrumbStrategy>(strategyKey);

        return strategy is null
            ? throw new InvalidOperationException(
                $"No breadcrumb strategy registered for key '{strategyKey}'.")
            : strategy;
    }
}
