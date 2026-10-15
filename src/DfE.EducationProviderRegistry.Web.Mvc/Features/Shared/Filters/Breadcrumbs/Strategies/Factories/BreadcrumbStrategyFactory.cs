using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies.Factories;

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
