using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies.Factories;

public interface IBreadcrumbStrategyFactory
{
    IBreadcrumbStrategy GetStrategy(BreadcrumbRouteRule rule);
}
