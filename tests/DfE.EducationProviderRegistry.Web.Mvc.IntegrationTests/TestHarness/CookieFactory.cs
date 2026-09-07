using System.Net;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness;

internal static class CookieFactory
{
    internal static Cookie AnalyticsCookie(Uri domain, bool analyticsValue)
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