using AngleSharp.Html.Dom;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness;
using DfE.EducationProviderRegistry.Web.Mvc.Settings;
using DfE.EducationProviderRegistry.Web.SharedTests.AngleSharp.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.Testing.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net;
using DfE.EducationProviderRegistry.Web.SharedTests.WebApplicationFactory.Extensions;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Analytics.GoogleTagManger;

public sealed class GoogleTagManagerTests
{
    [Fact]
    public async Task GoogleAnalyticsSettings_Disabled_Does_Not_Render_TagManager()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        using WebApplicationFactory<Program> factory =
            new WebApplicationFactory<Program>()
                .WithWebHostBuilder((builder) =>
                    builder.ConfigureServices((services)
                        => services.WithDisabledGoogleTagManager()));

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
    public async Task No_AnalyticsConsent_Choice_Does_Not_Render_TagManager()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        using WebApplicationFactory<Program> factory =
            new WebApplicationFactory<Program>()
                .WithWebHostBuilder((builder) =>
                    builder.ConfigureServices((services)
                        => services.WithGoogleTagManager()));

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
    public async Task Renders_TagManager_When_AnalyticsConsent_Given(bool analyticsConsent)
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        using WebApplicationFactory<Program> factory =
            new WebApplicationFactory<Program>()
                .WithWebHostBuilder((builder) =>
                    builder.ConfigureServices((services)
                        => services.WithGoogleTagManager()));

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

