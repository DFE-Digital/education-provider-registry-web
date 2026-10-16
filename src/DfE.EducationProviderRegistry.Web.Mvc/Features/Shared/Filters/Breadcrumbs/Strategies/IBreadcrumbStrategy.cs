using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies;

public interface IBreadcrumbStrategy
{
    void Apply(
        BreadcrumbJourney journey,
        BreadcrumbRouteRule rule,
        BreadcrumbItem currentBreadcrumbItem);
}
