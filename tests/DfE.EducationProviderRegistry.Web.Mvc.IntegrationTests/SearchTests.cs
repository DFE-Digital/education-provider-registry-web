using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Provider;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests;

public sealed class SearchTests
{
    private readonly CancellationToken _ct;
    private readonly IPostgresContainerConnectionStringProvider _provider;

    public SearchTests(IPostgresContainerConnectionStringProvider provider)
    {
        _ct = TestContext.Current.CancellationToken;
        _provider = provider;
    }

    [Fact]
    public async Task Test()
    {
        // Arrange
        using EducationProviderRegistryWebApplicationFactory factory = new(_provider);
        using HttpClient client = factory.CreateDefaultedHttpClient();

        // Act
        HttpResponseMessage response = await client.GetAsync("/search", _ct);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType!.ToString());
    }
}