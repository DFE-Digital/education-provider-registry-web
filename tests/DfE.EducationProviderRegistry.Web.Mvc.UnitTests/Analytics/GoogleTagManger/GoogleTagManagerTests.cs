using AngleSharp.Html.Dom;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness;
using DfE.EducationProviderRegistry.Web.Mvc.Settings;
using DfE.EducationProviderRegistry.Web.SharedTests.AngleSharp.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.Testing.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Analytics.GoogleTagManger;

public sealed class GoogleTagManagerTests
{
    [Fact]
    public async Task GoogleAnalyticsSettings_Disabled_Does_Not_Display_TagManager_In_Html()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        using WebApplicationFactory<Program> factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder((builder) =>
            {
                builder.ConfigureServices((services) =>
                {
                    services.PostConfigure<GoogleAnalyticsSettings>((opts) =>
                    {
                        // Tag manager disabled
                        opts.ContainerId = string.Empty;
                    });
                });
            });

        using HttpClient client = factory.CreateClient();

        // Act
        using HttpResponseMessage response = await client.GetAsync("/", ct);

        // Assert
        using IHtmlDocument doc = await response.AssertSuccessfulHtmlResponseAsync();

        GoogleAnalyticsSettings settings =
            factory.Services.GetRequiredService<
                IOptions<GoogleAnalyticsSettings>>().Value;

        GoogleAnalyticsComponent component = new(doc, settings);
        Assert.False(component.Exists());
    }

    [Fact]
    public async Task GET_Given_No_Analytics_Consent_Choice_Then_Does_Not_Render_TagManager()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        using WebApplicationFactory<Program> factory = new();

        using HttpClient client = factory.CreateClient();

        // Act
        using HttpResponseMessage response = await client.GetAsync("/", ct);

        // Assert
        using IHtmlDocument doc = await response.AssertSuccessfulHtmlResponseAsync();

        GoogleAnalyticsSettings settings =
            factory.Services.GetRequiredService<
                IOptions<GoogleAnalyticsSettings>>().Value;

        GoogleAnalyticsComponent component = new(doc, settings);
        Assert.False(component.Exists());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GET_When_Analytics_Only_When_Consent_Granted_Renders_GoogleTagManager(bool analyticsConsent)
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        using WebApplicationFactory<Program> factory = new();

        CookieContainer cookieContainer = new();

        cookieContainer.Add(
            CookieFactory.AnalyticsCookie(
                domain: factory.Server.BaseAddress,
                analyticsValue: analyticsConsent));

        CookieContainerHandler handler = new(cookieContainer);

        using HttpClient client = factory.CreateDefaultClient(handler);

        // Act
        using HttpResponseMessage response = await client.GetAsync("/", ct);

        // Assert
        using IHtmlDocument doc = await response.AssertSuccessfulHtmlResponseAsync();

        GoogleAnalyticsSettings settings =
            factory.Services.GetRequiredService<
                IOptions<GoogleAnalyticsSettings>>().Value;

        GoogleAnalyticsComponent component = new(doc, settings);

        if (analyticsConsent)
        {
            // Exists
            Assert.True(component.Exists());
        }
        else
        {
            Assert.False(component.Exists());
        }
    }
}

