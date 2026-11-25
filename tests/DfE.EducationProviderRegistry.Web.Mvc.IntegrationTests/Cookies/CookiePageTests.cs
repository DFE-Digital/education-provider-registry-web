using DfE.Document.Anglesharp;
using DfE.EducationProviderRegistry.Web.PageComponents;
using DfE.EducationProviderRegistry.Web.PageComponents.Pages;
using Microsoft.AspNetCore.TestHost;


namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Cookies;

public sealed class CookiesPageTests
{
    [Fact]
    public async Task GET_CookiesPage_With_No_Cookie_Has_No_Selection()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        using WebApplicationFactory<Program> factory = new();
        using HttpClient client = factory.CreateClient();

        // Act
        using HttpResponseMessage response = await client.GetAsync("/cookies", ct);

        // Assert
        using IHtmlDocument document = await response.AssertSuccessfulHtmlResponseAsync();

        CookiesPageAnalyticsFormComponent page = new(SearchContext.From(document));

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

        using WebApplicationFactory<Program> factory =
            new WebApplicationFactory<Program>()
                .WithWebHostBuilder((builder) =>
                    builder.ConfigureTestServices((services)
                        => services.AddTestAntiForgeryTokenServices()));

        // Must be set else Secure Cookies are not sent
        factory.ClientOptions.BaseAddress = new("https://localhost");

        using HttpClient client = factory.CreateClient();

        AntiForgeryContext antiForgery = await client.GetAntiforgeryTokensAsync(ct);

        HttpResponseMessage initialResponse = await client.GetAsync("/cookies", ct);

        CookiesPageAnalyticsFormComponent page =
            new(
                SearchContext.From(
                    await initialResponse.AssertSuccessfulHtmlResponseAsync()));

        HtmlForm form =
            page.GetForm(analytics)
                .AddField(antiForgery.FormFieldName, antiForgery.RequestToken);

        HttpRequestMessage request = form.ToHttpRequestMessage();
        request.Headers.Add("Cookie", antiForgery.CookieHeader);

        // Act
        using HttpResponseMessage response = await client.SendAsync(request, ct);

        // Assert
        using IHtmlDocument responseDocument = await response.AssertSuccessfulHtmlResponseAsync();

        CookiesPageAnalyticsFormComponent updatedPage = new(SearchContext.From(responseDocument));

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
