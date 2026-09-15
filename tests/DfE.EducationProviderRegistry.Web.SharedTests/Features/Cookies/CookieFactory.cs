using System.Net;

namespace DfE.EducationProviderRegistry.Web.SharedTests.Features.Cookies;

public static class CookieFactory
{
    public static Cookie AnalyticsCookie(Uri domain, bool analyticsValue)
    {
        return new Cookie(
            name: "cookies_policy",
            value: Uri.EscapeDataString(
                $$"""
                    {
                        "analytics":{{analyticsValue.ToString().ToLowerInvariant()}}
                    }
                """
            ))
        {
            Domain = domain.Host
        };
    }
}