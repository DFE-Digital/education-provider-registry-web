namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Shared;

public static class HttpContextNavigationHelpers
{
    public static bool IsDirectNavigation(HttpContext context)
    {
        // Note: the referrer header is not guaranteed to be sent by browsers
        // This is a heuristic determining if request originated from our host
        string? refererHeader = context.Request.Headers.Referer;

        if (string.IsNullOrWhiteSpace(refererHeader))
        {
            return true;
        }

        if (!Uri.TryCreate(refererHeader, UriKind.Absolute, out Uri? refererUri))
        {
            return true;
        }

        return !refererUri.Host.Equals(context.Request.Host.Host, StringComparison.OrdinalIgnoreCase);
    }
}
