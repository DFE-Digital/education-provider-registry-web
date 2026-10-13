using DfE.EducationProviderRegistry.Web.Mvc.Middleware.Breadcrumbs.Options;

namespace DfE.EducationProviderRegistry.Web.Mvc.Middleware.Breadcrumbs.Routing;

public interface IBreadcrumbRouteResolver
{
    bool TryResolveRule(PathString path, out BreadcrumbRouteRule? rule);
}
