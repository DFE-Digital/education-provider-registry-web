using AngleSharp.Html.Dom;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Extensions;
using Microsoft.AspNetCore.Mvc.Testing.Handlers;
using System.Net;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Cookies;

public sealed class CookiesTests : WebApplicationFactoryBaseIntegrationTest
{
    public CookiesTests(IServiceProvider provider) : base(provider)
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

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Submit_CookieBanner_With_AntiForgeryTokenAndCookie_Succeeds_With_Redirect(bool analytics)
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        using HttpClient client = Factory.CreateClient();
        using HttpResponseMessage cookieBannerHttpResponse = await client.GetAsync("/", ct);
        CookieBanner cookieBanner = new(document: await cookieBannerHttpResponse.AssertSuccessfulHtmlResponseAsync());

        // Create HttpRequest and add AntiForgery request token
        using HttpRequestMessage request =
            cookieBanner.CreateSubmitCookieBannerHttpRequest(addAntiForgeryToken: true, analytics: analytics);

        request.AddAspNetCoreAntiForgeryCookie(cookieBannerHttpResponse);

        // Act
        using HttpResponseMessage response = await client.SendAsync(request, ct);

        // Assert
        Assert.Equal(HttpStatusCode.Found, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        Assert.Equal(
            expected: new Uri($"/cookies?saved=true", UriKind.Relative),
            actual: response.Headers.Location);

        // Assert Set-Cookie is returned setting the analytics choice
        const string analyticsCookiePrefix = "cookies_policy=";

        string cookie = response.Headers
            .GetValues("Set-Cookie")
            .Single(x => x.StartsWith(analyticsCookiePrefix));

        Assert.Contains(analytics ? "true" : "false", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("secure", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("httponly", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("samesite=", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("path=/", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("expires=", cookie, StringComparison.OrdinalIgnoreCase);

        // Assert redirect is valid HTTP route
        using HttpResponseMessage redirectedResponse =
            await client.GetAsync(response.Headers.Location, ct);

        await redirectedResponse.AssertSuccessfulHtmlResponseAsync();
    }

    [Fact]
    public async Task Submit_CookieBanner_Requests_Without_AntiForgery_Cookie_Is_Rejected()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        using HttpClient client = Factory.CreateClient();
        using HttpResponseMessage cookieBannerHttpResponse = await client.GetAsync("/", ct);
        CookieBanner cookieBanner = new(document: await cookieBannerHttpResponse.AssertSuccessfulHtmlResponseAsync());

        using HttpRequestMessage request = cookieBanner.CreateSubmitCookieBannerHttpRequest(addAntiForgeryToken: true);

        // Act
        using HttpResponseMessage response = await client.SendAsync(request, ct);

        // Assert
        // note: app.UseStatusCodePagesWithReExecute("/not-found"); re-executes a failed response (BadRequest)
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("/", true)]
    [InlineData("/cookies", true)]
    [InlineData("/cookies", false)]
    [InlineData("/search", false)]
    public async Task GET_Any_Route_With_Analytics_Cookie_Does_Not_Display_Cookie_Banner(string path, bool analytics)
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        CookieContainer cookieContainer = new();

        cookieContainer.Add(
            AnalyticsCookie(
                Factory.Server.BaseAddress,
                analytics));

        CookieContainerHandler handler = new(cookieContainer);

        using HttpClient client = Factory.CreateDefaultClient(handler);

        // Act
        using HttpResponseMessage response = await client.GetAsync(path, ct);

        // Assert
        using IHtmlDocument doc = await response.AssertSuccessfulHtmlResponseAsync();
        CookieBanner banner = new(doc);
        Assert.False(banner.Exists());
    }

    private static Cookie AnalyticsCookie(Uri domain, bool analytics)
    {
        return new Cookie(
            name: "cookies_policy",
            value: Uri.EscapeDataString(
                $$"""
                    {
                        "analytics":{{analytics.ToString().ToLowerInvariant()}}
                    }
                """
            ))
        {
            Path = "/",
            Domain = domain.Host
        };
    }

    // Invalid JSON errors and displays cookie policy
}