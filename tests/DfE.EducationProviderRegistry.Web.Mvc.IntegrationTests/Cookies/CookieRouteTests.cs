using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness.Antiforgery;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness.Antiforgery.Extensions;
using System.Net;
using HttpMethod = System.Net.Http.HttpMethod;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Cookies;

public sealed class CookiesControllerTests : WebApplicationFactoryBaseTest
{
    public CookiesControllerTests(IServiceProvider provider) : base(provider)
    {
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Save_With_Valid_Request_Sets_Cookie_And_Redirects(bool analytics)
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        using HttpClient client = Factory.CreateClient();

        AntiForgeryContext antiforgery = await client.GetAntiforgeryTokensAsync(ct);

        using HttpRequestMessage request = new(HttpMethod.Post, "/cookies")
        {
            Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    ["analytics"] = analytics ? "true" : "false",
                    [antiforgery.FormFieldName] = antiforgery.RequestToken
                })
        };

        request.Headers.Add("Cookie", antiforgery.CookieHeader);

        // Act
        using HttpResponseMessage response =
            await client.SendAsync(request, ct);

        // Assert
        Assert.Equal(
            HttpStatusCode.Found,
            response.StatusCode);

        Assert.Equal(
            new Uri("/cookies?saved=true", UriKind.Relative),
            response.Headers.Location);

        const string analyticsCookiePrefix = "cookies_policy=";

        string cookie =
            response.Headers
                .GetValues("Set-Cookie")
                .Single(x =>
                    x.StartsWith(
                        analyticsCookiePrefix,
                        StringComparison.OrdinalIgnoreCase));

        Assert.Contains(
            analytics ? "true" : "false",
            cookie,
            StringComparison.OrdinalIgnoreCase);

        Assert.Contains(
            "secure",
            cookie,
            StringComparison.OrdinalIgnoreCase);

        Assert.Contains(
            "httponly",
            cookie,
            StringComparison.OrdinalIgnoreCase);

        Assert.Contains(
            "samesite=",
            cookie,
            StringComparison.OrdinalIgnoreCase);

        Assert.Contains(
            "path=/",
            cookie,
            StringComparison.OrdinalIgnoreCase);

        Assert.Contains(
            "expires=",
            cookie,
            StringComparison.OrdinalIgnoreCase);
    }
}