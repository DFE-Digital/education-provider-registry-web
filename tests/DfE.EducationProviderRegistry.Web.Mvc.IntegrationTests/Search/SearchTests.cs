using DfE.Core.Libraries.IntegrationTests.Database.Abstractions;
using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Providers;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Search;

public sealed class SearchTests : IAsyncLifetime
{
    private IDatabase? _db;
    private readonly CancellationToken _ct;
    private readonly IPostgresDatabaseProvider _dbProvider;
    private string? _postgresConnectionString;

    public SearchTests(
        IPostgresDatabaseProvider dbProvider)
    {
        _ct = TestContext.Current.CancellationToken;
        _dbProvider = dbProvider;
    }

    public async ValueTask InitializeAsync()
    {
        _db = await _dbProvider.GetDatabaseAsync("postgres", _ct);
        await _db.StartAsync(_ct);

        _postgresConnectionString = await _dbProvider.GetConnectionStringAsync("postgres", cancellationToken: _ct);
    }

    public async ValueTask DisposeAsync()
    {
        if (_db is not null)
        {
            await _db.DisposeAsync();
        }
    }

    [Fact]
    public async Task Search_With_Identity_Term_Returns_Results()
    {
        // Arrange
        using EducationProviderRegistryWebApplicationFactory factory = new(_postgresConnectionString!);

        using HttpClient client = factory.CreateDefaultedHttpClient();
        HttpResponseMessage pageResponse = await client.GetAsync("/search", _ct);
        SearchPanel panel = new(await HtmlHelpers.GetDocumentAsync(pageResponse));

        HttpRequestMessage message =
            SearchHttpRequestBuilder.Create(panel)
                .WithIdentitySearchTerm("School")
                .Build();
        // Act
        HttpResponseMessage response = await client.SendAsync(message, _ct);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType!.ToString());
    }
}

