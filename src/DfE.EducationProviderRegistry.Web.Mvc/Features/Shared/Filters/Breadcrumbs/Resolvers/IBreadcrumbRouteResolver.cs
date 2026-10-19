using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Resolvers;

public interface IBreadcrumbRouteResolver
{
    bool TryResolveRule(PathString path, out BreadcrumbRouteRule? rule);
}
