using System.Reflection;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Resolvers;

public sealed class BreadcrumbTitleResolver : IBreadcrumbTitleResolver
{
    public string Resolve(BreadcrumbRouteRule rule, ViewResult viewResult)
    {
        if (!string.IsNullOrWhiteSpace(rule.Title))
            return rule.Title;

        if (!string.IsNullOrWhiteSpace(rule.TitleModelProperty))
        {
            object? model =
                viewResult.Model;

            PropertyInfo? property =
                model?.GetType().GetProperty(rule.TitleModelProperty);

            return property?.GetValue(model)?.ToString()
                   ?? string.Empty;
        }

        return string.Empty;
    }
}
