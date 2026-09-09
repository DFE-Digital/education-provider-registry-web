using DfE.EducationProviderRegistry.Web.Mvc.Settings;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace DfE.EducationProviderRegistry.Web.Mvc.Tests.Settings;

public class GoogleAnalyticsSettingsTests
{
    [Fact]
    public void Domain_ReturnsGoogleTagManagerBaseUrl()
    {
        var settings = new GoogleAnalyticsSettings();

        Assert.Equal(
            "https://www.googletagmanager.com",
            settings.Domain);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("auth", null)]
    [InlineData(null, "env-3")]
    [InlineData("", "env-3")]
    [InlineData("auth", "")]
    [InlineData(" ", "env-3")]
    [InlineData("auth", " ")]
    public void TagManagerQueryString_WhenCredentialsAreIncomplete_ReturnsEmpty(
        string? authenticationId,
        string? previewValue)
    {
        var settings = new GoogleAnalyticsSettings
        {
            AuthenticationId = authenticationId,
            PreviewValue = previewValue
        };

        Assert.Equal(string.Empty, settings.TagManagerQueryString);
    }

    [Fact]
    public void TagManagerQueryString_WhenCredentialsAreProvided_ReturnsEncodedQueryString()
    {
        var settings = new GoogleAnalyticsSettings
        {
            AuthenticationId = "a+b&c",
            PreviewValue = "env 3"
        };

        string result = settings.TagManagerQueryString;

        Assert.Equal(
            "&gtm_auth=a%2Bb%26c&gtm_preview=env%203&gtm_cookies_win=x",
            result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void IsGoogleTagManagerEnabled_WhenContainerIdIsMissing_ReturnsFalse(
        string? containerId)
    {
        var settings = new GoogleAnalyticsSettings
        {
            ContainerId = containerId
        };

        var context = CreateContext("yes");

        Assert.False(settings.IsGoogleTagManagerEnabled(context));
    }

    [Theory]
    [InlineData("yes", true)]
    [InlineData("no", false)]
    [InlineData(null, false)]
    public void IsGoogleTagManagerEnabled_ReturnsExpectedResultForAnalyticsConsent(
        string? consentValue,
        bool expected)
    {
        var settings = new GoogleAnalyticsSettings
        {
            ContainerId = "GTM-TEST123"
        };

        var context = CreateContext(consentValue);

        Assert.Equal(
            expected,
            settings.IsGoogleTagManagerEnabled(context));
    }

    [Fact]
    public void IsGoogleTagManagerEnabled_WhenContextIsNull_ReturnsFalse()
    {
        var settings = new GoogleAnalyticsSettings
        {
            ContainerId = "GTM-TEST123"
        };

        Assert.False(settings.IsGoogleTagManagerEnabled(null!));
    }

    private static DefaultHttpContext CreateContext(string? consentValue)
    {
        var context = new DefaultHttpContext();

        if (consentValue is not null)
        {
            context.Request.Headers.Cookie =
                $"cookies_analytics={consentValue}";
        }

        return context;
    }
}