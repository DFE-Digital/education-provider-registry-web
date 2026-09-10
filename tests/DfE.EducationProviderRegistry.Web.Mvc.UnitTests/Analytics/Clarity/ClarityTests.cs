using AngleSharp.Html.Dom;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness;
using DfE.EducationProviderRegistry.Web.SharedTests.AngleSharp.Extensions;
using DfE.EducationProviderRegistry.Web.SharedTests.WebApplicationFactory.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.Testing.Handlers;
using System.Net;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Analytics.Clarity;

public sealed class ClarityTests
{

    [Fact]
    public async Task ClaritySettings_Disabled_Does_Not_Render_Clarity()
    {
        // Arrange
        using WebApplicationFactory<Program> factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder((builder) =>
                builder.ConfigureServices((services)
                    => services.WithDisabledClarity()));

        CancellationToken ct = TestContext.Current.CancellationToken;

        using HttpClient client = factory.CreateClient();

        // Act
        using HttpResponseMessage response = await client.GetAsync("/", ct);

        // Assert
        using IHtmlDocument doc = await response.AssertSuccessfulHtmlResponseAsync();

        string html = doc.DocumentElement.OuterHtml;

        Assert.DoesNotContain(ClarityConstants.JavascriptFuncDefinition, html);
        Assert.DoesNotContain(ClarityConstants.RunJavascriptFunc, html);
    }

    [Fact]
    public async Task No_AnalyticsConsent_Choice_Does_Not_Run_Clarity()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        using WebApplicationFactory<Program> factory = new WebApplicationFactory<Program>()
                    .WithWebHostBuilder((builder) =>
                        builder.ConfigureServices((services)
                            => services.WithClarity()));

        using HttpClient client = factory.CreateClient();

        // Act
        using HttpResponseMessage response = await client.GetAsync("/", ct);

        // Assert
        using IHtmlDocument doc = await response.AssertSuccessfulHtmlResponseAsync();

        string html = doc.DocumentElement.OuterHtml;

        Assert.Contains(ClarityConstants.JavascriptFuncDefinition, html);
        Assert.DoesNotContain(ClarityConstants.RunJavascriptFunc, html);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Runs_Clarity_When_AnalyticsConsent_Given(bool analyticsConsent)
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

        string html = doc.DocumentElement.OuterHtml;

        // Definition
        Assert.Contains(ClarityConstants.JavascriptFuncDefinition, html);

        if (analyticsConsent)
        {
            // Is executed
            Assert.Contains(ClarityConstants.RunJavascriptFunc, html);
        }
        else
        {
            // Is not executed
            Assert.DoesNotContain(ClarityConstants.RunJavascriptFunc, html);
        }
    }
}