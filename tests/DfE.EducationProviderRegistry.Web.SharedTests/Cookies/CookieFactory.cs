using System.Net;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness;

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