using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness.Anglesharp;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness.Anglesharp.Extensions;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness.Antiforgery;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness.Antiforgery.Extensions;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Cookies;

public sealed class CookiesPageTests : WebApplicationFactoryBaseTest
{
    public CookiesPageTests(IServiceProvider provider) : base(provider)
    {
    }

    [Fact]
    public async Task GET_CookiesPage_With_No_Cookie_Has_No_Selection()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        using HttpClient client = Factory.CreateClient();

        // Act
        using HttpResponseMessage response = await client.GetAsync("/cookies", ct);

        // Assert
        using IHtmlDocument document = await response.AssertSuccessfulHtmlResponseAsync();

        CookiesPageAnalyticsForm page = new(document);

        Assert.False(page.AcceptAnalyticsSelected());
        Assert.False(page.RejectAnalyticsSelected());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Submit_CookiesPage_With_AntiForgeryTokenAndCookie_Succeeds_And_Displays_Current_Selection(bool analytics)
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;
        Factory.ClientOptions.AllowAutoRedirect = true;
        using HttpClient client = Factory.CreateClient();
        
        AntiForgeryContext antiForgery = await client.GetAntiforgeryTokensAsync(ct);

        CookiesPageAnalyticsForm page = 
            new(
                await (
                    await client.GetAsync("/cookies", ct))
                        .AssertSuccessfulHtmlResponseAsync());

        HtmlForm form = 
            page.GetForm(analytics)
                .AddField(antiForgery.FormFieldName, antiForgery.RequestToken);

        HttpRequestMessage request = form.ToHttpRequestMessage();
        request.Headers.Add("Cookie", antiForgery.CookieHeader);

        // Act
        using HttpResponseMessage response = await client.SendAsync(request, ct);

        // Assert
        using IHtmlDocument document = await response.AssertSuccessfulHtmlResponseAsync();

        CookiesPageAnalyticsForm updatedPage = new(document);

        if (analytics)
        {
            Assert.True(updatedPage.AcceptAnalyticsSelected());
            Assert.False(updatedPage.RejectAnalyticsSelected());
        }
        else
        {
            Assert.False(updatedPage.AcceptAnalyticsSelected());
            Assert.True(updatedPage.RejectAnalyticsSelected());
        }
    }
}