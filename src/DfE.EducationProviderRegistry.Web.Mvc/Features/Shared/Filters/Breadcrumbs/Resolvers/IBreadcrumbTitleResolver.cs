using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Resolvers;

public interface IBreadcrumbTitleResolver
{
    string Resolve(
        BreadcrumbRouteRule rule,
        ViewResult viewResult);
}
