using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies;

public sealed class BaseJourneyStrategy : IBreadcrumbStrategy
{
    public void Apply(BreadcrumbJourney journey, BreadcrumbRouteRule rule, string currentUrl)
    {
        ArgumentNullException.ThrowIfNull(journey);
        ArgumentNullException.ThrowIfNull(rule);

        journey.Items =
        [
            .. (rule.BaseJourney ?? []).Select(
                breadcrumbPrefixItem =>
                    new BreadcrumbItem(
                        breadcrumbPrefixItem.Title,
                        breadcrumbPrefixItem.Url))
        ];

        journey.Items.Add(new BreadcrumbItem(rule.Title!, currentUrl));
    }
}
