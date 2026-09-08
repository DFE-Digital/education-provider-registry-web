using AngleSharp.Html.Dom;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness.Anglesharp.Extensions;
using DfE.EducationProviderRegistry.Web.Mvc.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Analytics;

public sealed class ClaritySettingsTests : WebApplicationFactoryBaseTest
{
    public ClaritySettingsTests(IServiceProvider provider) : base(provider)
    {
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        services.PostConfigure<ClaritySettings>((opts) =>
        {
            opts.Enabled = false;
        });
    }

    [Fact]
    public async Task ClaritySettings_Disabled_Does_Not_Display_Clarity_In_Html()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        using HttpClient client = Factory.CreateClient();

        // Act
        using HttpResponseMessage response = await client.GetAsync("/", ct);

        // Assert
        using IHtmlDocument doc = await response.AssertSuccessfulHtmlResponseAsync();

        string html = doc.DocumentElement.OuterHtml;

        Assert.DoesNotContain(ClarityConstants.JavascriptFuncDefinition, html);
        Assert.DoesNotContain(ClarityConstants.RunJavascriptFunc, html);
    }
}