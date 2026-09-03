using DfE.EducationProviderRegistry.Web.Mvc.Extensions;
using DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Extensions.TestDoubles;
using Microsoft.AspNetCore.Http;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Extensions;

public sealed class AnalyticsConsentExtensionsTests
{
    [Fact]
    public void ReturnsFalse_WhenCookieIsMissing()
    {
        DefaultHttpContext context = CookieFactoryTestDouble.CreateContextWithCookie(null!);

        bool result = AnalyticsConsentExtensions.IsAnalyticsConsentGranted(context);

        Assert.False(result);
    }

    [Fact]
    public void ReturnsFalse_WhenCookieIsEmpty()
    {
        DefaultHttpContext context = CookieFactoryTestDouble.CreateContextWithCookie(string.Empty);

        bool result = AnalyticsConsentExtensions.IsAnalyticsConsentGranted(context);

        Assert.False(result);
    }

    [Fact]
    public void ReturnsFalse_WhenCookieIsWhitespace()
    {
        DefaultHttpContext context = CookieFactoryTestDouble.CreateContextWithCookie("   ");

        bool result = AnalyticsConsentExtensions.IsAnalyticsConsentGranted(context);

        Assert.False(result);
    }

    [Fact]
    public void ReturnsFalse_WhenCookieIsMalformedJson()
    {
        string encoded = Uri.EscapeDataString("{not valid json}");
        DefaultHttpContext context = CookieFactoryTestDouble.CreateContextWithCookie(encoded);

        bool result = AnalyticsConsentExtensions.IsAnalyticsConsentGranted(context);

        Assert.False(result);
    }

    [Fact]
    public void ReturnsFalse_WhenAnalyticsPropertyIsMissing()
    {
        string encoded = Uri.EscapeDataString("{\"somethingElse\": true}");
        DefaultHttpContext context = CookieFactoryTestDouble.CreateContextWithCookie(encoded);

        bool result = AnalyticsConsentExtensions.IsAnalyticsConsentGranted(context);

        Assert.False(result);
    }

    [Fact]
    public void ReturnsFalse_WhenAnalyticsPropertyIsFalse()
    {
        string encoded = Uri.EscapeDataString("{\"analytics\": false}");
        DefaultHttpContext context = CookieFactoryTestDouble.CreateContextWithCookie(encoded);

        bool result = AnalyticsConsentExtensions.IsAnalyticsConsentGranted(context);

        Assert.False(result);
    }

    [Fact]
    public void ReturnsTrue_WhenAnalyticsPropertyIsTrue()
    {
        string encoded = Uri.EscapeDataString("{\"analytics\": true}");
        DefaultHttpContext context = CookieFactoryTestDouble.CreateContextWithCookie(encoded);

        bool result = AnalyticsConsentExtensions.IsAnalyticsConsentGranted(context);

        Assert.True(result);
    }

    [Fact]
    public void ReturnsFalse_WhenAnalyticsPropertyIsNotBoolean()
    {
        string encoded = Uri.EscapeDataString("{\"analytics\": \"yes\"}");
        DefaultHttpContext context = CookieFactoryTestDouble.CreateContextWithCookie(encoded);

        bool result = AnalyticsConsentExtensions.IsAnalyticsConsentGranted(context);

        Assert.False(result);
    }

    [Fact]
    public void ReturnsFalse_WhenAnalyticsPropertyIsNull()
    {
        string encoded = Uri.EscapeDataString("{\"analytics\": null}");
        DefaultHttpContext context = CookieFactoryTestDouble.CreateContextWithCookie(encoded);

        bool result = AnalyticsConsentExtensions.IsAnalyticsConsentGranted(context);

        Assert.False(result);
    }
}
