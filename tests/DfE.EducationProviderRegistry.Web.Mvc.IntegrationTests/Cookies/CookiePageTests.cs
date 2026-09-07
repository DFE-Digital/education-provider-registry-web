using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness.Anglesharp;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness.Anglesharp.Extensions;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness.Antiforgery;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness.Antiforgery.Extensions;
using Docker.DotNet.Handler.Abstractions;
using Microsoft.AspNetCore.Mvc.Testing.Handlers;
using System.Diagnostics;
using System.Net;

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
    public async Task Submit_CookiesPage_Without_AntiForgery_Cookie_Is_Rejected(bool analytics)
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        using HttpClient client = Factory.CreateClient();

        using HttpResponseMessage pageResponse = await client.GetAsync("/cookies", ct);

        CookiesPageAnalyticsForm page = new(await pageResponse.AssertSuccessfulHtmlResponseAsync());

        HtmlForm form = page.GetForm(analytics: analytics);

        using HttpRequestMessage request = form.ToHttpRequestMessage();

        // Act
        using HttpResponseMessage response = await client.SendAsync(request, ct);

        // Assert
        // note: app.UseStatusCodePagesWithReExecute("/not-found");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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
        // Request must be over HTTPS as Cookie is set to Strict
        client.BaseAddress = new("https://localhost");

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