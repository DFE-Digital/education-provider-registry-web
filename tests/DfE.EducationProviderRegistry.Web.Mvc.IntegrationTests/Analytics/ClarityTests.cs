using AngleSharp.Html.Dom;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness.Anglesharp.Extensions;
using DfE.EducationProviderRegistry.Web.Mvc.Settings;
using Microsoft.AspNetCore.Mvc.Testing.Handlers;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Analytics;

public sealed class ClarityTests : WebApplicationFactoryBaseTest
{
    public ClarityTests(IServiceProvider provider) : base(provider)
    {
    }

    [Fact]
    public async Task GET_When_Analytics_Consent_No_Choice_Made_Does_Not_Auto_Load_Clarity()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        using HttpClient client = Factory.CreateClient();

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
    public async Task GET_When_Analytics_Only_When_Consent_Granted_Auto_Loads_Clarity(bool analyticsConsent)
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        CookieContainer cookieContainer = new();

        cookieContainer.Add(
            CookieFactory.AnalyticsCookie(
                domain: Factory.Server.BaseAddress,
                analyticsValue: analyticsConsent));

        CookieContainerHandler handler = new(cookieContainer);

        using HttpClient client = Factory.CreateDefaultClient(handler);

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