namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Search;

public sealed class SearchTests : WebApplicationFactoryBaseIntegrationTest
{
    public SearchTests(IServiceProvider provider) : base(provider)
    {

    }

    [Fact]
    public async Task Search_With_Identity_Term_Returns_Results()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        using HttpClient client = Factory.CreateDefaultedHttpClient();
        HttpResponseMessage pageResponse = await client.GetAsync("/search", ct);

        SearchPanel panel = new(await HtmlHelpers.GetDocumentAsync(pageResponse));

        HttpRequestMessage message =
            SearchHttpRequestBuilder.Create(panel)
                .WithIdentitySearchTerm("School")
                .Build();
        // Act
        HttpResponseMessage response = await client.SendAsync(message, ct);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType!.ToString());
    }
}

