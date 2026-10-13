using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Middleware.Breadcrumbs.Options;

namespace DfE.EducationProviderRegistry.Web.Mvc.Middleware.Breadcrumbs.Strategies;

public interface IBreadcrumbStrategy
{
    void Apply(BreadcrumbJourney journey, BreadcrumbRouteRule rule, string currentUrl);
}
