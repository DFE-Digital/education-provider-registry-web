using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness.Anglesharp;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness.Anglesharp.Extensions;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness.Antiforgery;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness.Antiforgery.Extensions;
using System.Net;
using HttpMethod = System.Net.Http.HttpMethod;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.CookieBanner;

public sealed class CookieBannerTests : WebApplicationFactoryBaseTest
{
    public CookieBannerTests(IServiceProvider provider) : base(provider)
    {
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/cookies")]
    [InlineData("/search")]
    public async Task GET_Any_Route_Displays_Cookie_Banner(string path)
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;
        using HttpClient client = Factory.CreateClient();

        // Act
        using HttpResponseMessage response = await client.GetAsync(path, ct);

        // Assert
        using IHtmlDocument doc = await response.AssertSuccessfulHtmlResponseAsync();

        CookieBanner banner = new(doc);
        Assert.True(banner.Exists());
    }

    [Fact]
    public async Task Submit_CookieBanner_Requests_Without_AntiForgery_RequestToken_Is_Rejected()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        using HttpClient client = Factory.CreateClient();
        using HttpResponseMessage pageResponse = await client.GetAsync("/", ct);
        CookieBanner cookieBanner = new(await pageResponse.AssertSuccessfulHtmlResponseAsync());
        HtmlForm form = cookieBanner.GetForm(analytics: false);

        using HttpRequestMessage request = form.ToHttpRequestMessage();

        AntiForgeryContext antiForgery = await client.GetAntiforgeryTokensAsync(ct);
        request.Headers.Add("Cookie", antiForgery.CookieHeader);

        // Act
        using HttpResponseMessage response = await client.SendAsync(request, ct);

        // Assert
        // note: app.UseStatusCodePagesWithReExecute("/not-found");
        // re-executes a failed antiforgery response (BadRequest)
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("/", true)]
    [InlineData("/cookies", true)]
    [InlineData("/cookies", false)]
    [InlineData("/search", false)]
    public async Task GET_Any_Route_With_Analytics_Cookie_Does_Not_Display_Cookie_Banner(string path, bool enableAnalytics)
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        using HttpClient client = Factory.CreateClient();

        HttpRequestMessage request = new(HttpMethod.Get, path);

        request.Headers.Add
            ("Cookie",
            CookieFactory.AnalyticsCookie(Factory.Server.BaseAddress, enableAnalytics).ToString());

        // Act
        using HttpResponseMessage response = await client.SendAsync(request, ct);

        // Assert
        using IHtmlDocument doc = await response.AssertSuccessfulHtmlResponseAsync();
        CookieBanner banner = new(doc);
        Assert.False(banner.Exists());
    }
}