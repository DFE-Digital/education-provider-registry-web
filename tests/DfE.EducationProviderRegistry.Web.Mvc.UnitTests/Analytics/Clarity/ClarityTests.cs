using AngleSharp.Html.Dom;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness;
using DfE.EducationProviderRegistry.Web.Mvc.Settings;
using DfE.EducationProviderRegistry.Web.SharedTests.AngleSharp.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.Testing.Handlers;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Analytics.Clarity;

public sealed class ClarityTests
{

    [Fact]
    public async Task ClaritySettings_Disabled_Does_Not_Display_Clarity_In_Html()
    {
        // Arrange
        using WebApplicationFactory<Program> factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder((builder) =>
            {
                builder.ConfigureServices((services) =>
                {
                    services.PostConfigure<ClaritySettings>((opts) =>
                    {
                        opts.Enabled = false;
                    });
                });
            });

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
    public async Task GET_When_Analytics_Consent_No_Choice_Made_Does_Not_Auto_Load_Clarity()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        using WebApplicationFactory<Program> factory = new();

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
    public async Task GET_Given_No_Analytics_Consent_Choice_Then_Does_Not_Load_Clarity(bool analyticsConsent)
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