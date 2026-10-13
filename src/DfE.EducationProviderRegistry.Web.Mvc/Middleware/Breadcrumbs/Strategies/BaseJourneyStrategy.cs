using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Middleware.Breadcrumbs.Options;

namespace DfE.EducationProviderRegistry.Web.Mvc.Middleware.Breadcrumbs.Strategies;

public sealed class BaseJourneyStrategy : IBreadcrumbStrategy
{
    public void Apply(BreadcrumbJourney journey, BreadcrumbRouteRule rule, string currentUrl)
    {
        journey.Items =
            [
                .. rule.BaseJourney.Select(
                    breadcrumbPrefixItem =>
                        new BreadcrumbItem(
                            breadcrumbPrefixItem.Title,
                            breadcrumbPrefixItem.Url))
            ];

        journey.Items.Add(new BreadcrumbItem(rule.Title, currentUrl));
    }
}
