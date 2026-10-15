namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.NavigationEvaluators;

public sealed class NavigationEvaluator : INavigationEvaluator
{
    public bool IsDirectNavigation(HttpContext context)
    {
        string? referer = context.Request.Headers.Referer;

        return string.IsNullOrWhiteSpace(referer) ||
               !Uri.TryCreate(referer, UriKind.Absolute, out Uri? refererUri) ||
               !refererUri.Host.Equals(context.Request.Host.Host, StringComparison.OrdinalIgnoreCase);
    }
}
