using DfE.EducationProviderRegistry.Web.Mvc.Middleware.Breadcrumbs.Options;

namespace DfE.EducationProviderRegistry.Web.Mvc.Middleware.Breadcrumbs.Strategies.Factories;

public interface IBreadcrumbStrategyFactory
{
    IBreadcrumbStrategy GetStrategy(BreadcrumbRouteRule rule);
}
