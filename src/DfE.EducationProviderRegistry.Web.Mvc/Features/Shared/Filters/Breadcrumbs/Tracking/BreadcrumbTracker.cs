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
        _breadcrumbStrategyFactory = breadcrumbStrategyFactory ??
            throw new ArgumentNullException(nameof(breadcrumbStrategyFactory));
    }

    public void Track(
        BreadcrumbJourney journey,
        BreadcrumbRouteRule rule,
        string title,
        string currentUrl)
    {
        ArgumentNullException.ThrowIfNull(journey);
        ArgumentNullException.ThrowIfNull(rule);

        rule.Title = title;

        IBreadcrumbStrategy strategy =
            _breadcrumbStrategyFactory.GetStrategy(rule);

        strategy.Apply(journey, rule, currentUrl);
    }
}
