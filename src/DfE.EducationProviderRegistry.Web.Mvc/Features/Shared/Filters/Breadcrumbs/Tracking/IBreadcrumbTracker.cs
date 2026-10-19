using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Tracking;

public interface IBreadcrumbTracker
{
    void Track(
        BreadcrumbJourney journey,
        BreadcrumbRouteRule rule,
        BreadcrumbItem currentBreadcrumbItem);
}
