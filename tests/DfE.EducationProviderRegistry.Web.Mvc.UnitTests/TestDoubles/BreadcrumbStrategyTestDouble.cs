using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.TestDoubles;

public sealed class BreadcrumbStrategyTestDouble : IBreadcrumbStrategy
{
    public void Apply(
        BreadcrumbJourney journey,
        BreadcrumbRouteRule rule,
        BreadcrumbItem currentBreadcrumbItem)
    {
    }
}
