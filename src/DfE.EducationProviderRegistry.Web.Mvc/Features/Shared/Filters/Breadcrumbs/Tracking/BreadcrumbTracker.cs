using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies.Factories;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Tracking;

public sealed class BreadcrumbTracker : IBreadcrumbTracker
{
    private readonly IBreadcrumbStrategyFactory _breadcrumbStrategyFactory;

    public BreadcrumbTracker(IBreadcrumbStrategyFactory breadcrumbStrategyFactory)
    {
        ArgumentNullException.ThrowIfNull(breadcrumbStrategyFactory);
        _breadcrumbStrategyFactory = breadcrumbStrategyFactory;
    }

    public void Track(
        BreadcrumbJourney journey,
        BreadcrumbRouteRule rule,
        BreadcrumbItem currentBreadcrumbItem)
    {
        ArgumentNullException.ThrowIfNull(journey);
        ArgumentNullException.ThrowIfNull(rule);
        ArgumentNullException.ThrowIfNull(currentBreadcrumbItem);

        IBreadcrumbStrategy strategy = _breadcrumbStrategyFactory
            .GetStrategy(rule);

        strategy.Apply(journey, rule, currentBreadcrumbItem);
    }
}
