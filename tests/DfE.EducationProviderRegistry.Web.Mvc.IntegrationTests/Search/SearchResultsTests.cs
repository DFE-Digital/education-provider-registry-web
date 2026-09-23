using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Search.TestDoubles;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.Search;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Search;

public sealed class SearchResultsTests
{

    [Fact]
    public async Task Search_With_Identity_Term_Returns_Results()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        (IUseCase<SearchRequest, UseCaseResponse<SearchResponse>> useCase,
            SearchResponse searchResponse) =
                SearchUseCaseTestDoubles.StubResponse();

        using WebApplicationFactory<Program> factory = SearchWebApplicationFactoryProvider.CreateFactory(useCase);

        using HttpClient client = factory.CreateClient();

        using HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(factory.Server.BaseAddress)
                .WithIdentitySearchTerm("School")
                .Build();

        // Act
        HttpResponseMessage response = await client.SendAsync(message, ct);

        // Assert
        IHtmlDocument responseDocument = await response.AssertSuccessfulHtmlResponseAsync();
        SearchResultsComponent results = new(responseDocument);

        string resultsHeading = results.GetHeading();
        Assert.StartsWith("Search results for ", resultsHeading);
        Assert.EndsWith("\"School\"", resultsHeading);

        Assert.Equal($"{searchResponse.SearchProviderResults!.Count} results", results.GetTotalResults());
        AssertSearchResultsDisplayed(results, searchResponse);
    }

    [Fact]
    public async Task Search_With_Location_Term_Returns_Results()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        (IUseCase<SearchRequest, UseCaseResponse<SearchResponse>> useCase,
            SearchResponse searchResponse) =
                SearchUseCaseTestDoubles.StubResponse();

        using WebApplicationFactory<Program> factory = SearchWebApplicationFactoryProvider.CreateFactory(useCase);

        using HttpClient client = factory.CreateClient();

        using HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(factory.Server.BaseAddress)
                .WithLocationTerm("LN1")
                .Build();

        // Act
        HttpResponseMessage response = await client.SendAsync(message, ct);

        // Assert
        IHtmlDocument doc = await response.AssertSuccessfulHtmlResponseAsync();
        SearchResultsComponent results = new(doc);
        string resultsHeading = results.GetHeading();

        Assert.StartsWith($"Search results for ", resultsHeading);
        Assert.EndsWith($"\"LN1\"", resultsHeading);
        Assert.Equal($"{searchResponse.SearchProviderResults!.Count} results", results.GetTotalResults());
        AssertSearchResultsDisplayed(results, searchResponse);
    }

    [Fact]
    public async Task Search_With_Identity_And_Location_Returns_Intersecting_Results()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        (IUseCase<SearchRequest, UseCaseResponse<SearchResponse>> useCase,
            SearchResponse searchResponse) =
                SearchUseCaseTestDoubles.StubResponse();

        using WebApplicationFactory<Program> factory = SearchWebApplicationFactoryProvider.CreateFactory(useCase);

        using HttpClient client = factory.CreateClient();

        using HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(factory.Server.BaseAddress)
                .WithIdentitySearchTerm("sch")
                .WithLocationTerm("LN1")
                .Build();
        // Act
        using HttpResponseMessage response = await client.SendAsync(message, ct);

        // Assert
        IHtmlDocument doc = await response.AssertSuccessfulHtmlResponseAsync();
        SearchResultsComponent results = new(doc);

        string resultsHeading = results.GetHeading();
        Assert.StartsWith($"Search results for ", resultsHeading);
        Assert.EndsWith($"\"sch\"\n \"LN1\"", resultsHeading);
        Assert.Equal($"{searchResponse.SearchProviderResults!.Count} results", results.GetTotalResults());
        AssertSearchResultsDisplayed(results, searchResponse);
    }

    private static void AssertSearchResultsDisplayed(SearchResultsComponent results, SearchResponse response)
    {
        IReadOnlyList<SearchResult> searchResults = results.GetSearchResults();

        Assert.NotEmpty(searchResults);
        Assert.Equal(searchResults.Count, response.SearchProviderResults!.Count);

        List<SearchAggregateResult> responseExpectedResults = response.SearchProviderResults.SearchResultCollection.ToList();

        foreach (SearchAggregateResult current in responseExpectedResults)
        {
            var searchResult = searchResults.Single(
                (result) =>
                    result.Name!.Equals(current.Name.Value, StringComparison.OrdinalIgnoreCase));

            // TODO extend to other properties
            Assert.Equal(current.Name.Value, searchResult.Name);
        }
    }
}
