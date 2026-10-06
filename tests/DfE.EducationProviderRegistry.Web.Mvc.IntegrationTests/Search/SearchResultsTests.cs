using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Search.TestDoubles;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.PageComponents;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.Search;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.Search.Components;

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

        using WebApplicationFactory<Program> factory = WebApplicationFactoryProvider.CreateFactory(useCase);

        using HttpClient client = factory.CreateClient();

        using HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(factory.Server.BaseAddress)
                .WithIdentitySearchTerm("School")
                .Build();

        // Act
        using HttpResponseMessage response = await client.SendAsync(message, ct);

        // Assert
        using IHtmlDocument responseDocument = await response.AssertSuccessfulHtmlResponseAsync();
        SearchResultsComponent results = new(responseDocument);

        string resultsHeading = results.GetHeading();
        Assert.StartsWith("Search results for ", resultsHeading);
        Assert.EndsWith("\"School\"", resultsHeading);

        Assert.Equal($"{searchResponse.SearchProviderResults!.Count} results", results.GetTotalResultsLabel());
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

        using WebApplicationFactory<Program> factory = WebApplicationFactoryProvider.CreateFactory(useCase);

        using HttpClient client = factory.CreateClient();

        using HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(factory.Server.BaseAddress)
                .WithLocationTerm("LN1")
                .Build();

        // Act
        using HttpResponseMessage response = await client.SendAsync(message, ct);

        // Assert
        using IHtmlDocument doc = await response.AssertSuccessfulHtmlResponseAsync();
        SearchResultsComponent results = new(doc);
        string resultsHeading = results.GetHeading();

        Assert.StartsWith($"Search results for ", resultsHeading);
        Assert.EndsWith($"\"LN1\"", resultsHeading);
        Assert.Equal($"{searchResponse.SearchProviderResults!.Count} results", results.GetTotalResultsLabel());
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

        using WebApplicationFactory<Program> factory = WebApplicationFactoryProvider.CreateFactory(useCase);

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
        using IHtmlDocument doc = await response.AssertSuccessfulHtmlResponseAsync();
        SearchResultsComponent results = new(doc);

        string resultsHeading = results.GetHeading();
        Assert.StartsWith($"Search results for ", resultsHeading);
        Assert.EndsWith($"\"sch\"\n \"LN1\"", resultsHeading);
        Assert.Equal($"{searchResponse.SearchProviderResults!.Count} results", results.GetTotalResultsLabel());
        AssertSearchResultsDisplayed(results, searchResponse);
    }

    private static void AssertSearchResultsDisplayed(SearchResultsComponent results, SearchResponse response)
    {
        IReadOnlyList<SearchResult> actualSearchResults = results.GetSearchResults();

        Assert.NotEmpty(actualSearchResults);
        Assert.Equal(actualSearchResults.Count, response.SearchProviderResults!.Count);

        foreach (SearchAggregateResult currentResult in response.SearchProviderResults.SearchResultCollection)
        {
            (TextContent name, IReadOnlyDictionary<TextContent, TextContent> values) =
                IsEstablishmentSearchResult(currentResult) ?
                    MapToEstablishmentResultsTable(currentResult) :
                        MapToGroupResultTable(currentResult);

            SearchResult actualSearchResult =
                actualSearchResults.Single((result) =>
                    result.Name.Text.Equals(currentResult.Name.Value, StringComparison.OrdinalIgnoreCase));

            Assert.Equal(name, actualSearchResult.Name);
            Assert.Equivalent(values, actualSearchResult.Values);
        }
    }

    private static bool IsEstablishmentSearchResult(SearchAggregateResult useCaseResponse)
        => useCaseResponse.ProviderCategory.Category.Equals("establishment", StringComparison.OrdinalIgnoreCase);

    private static (TextContent, IReadOnlyDictionary<TextContent, TextContent>) MapToEstablishmentResultsTable(SearchAggregateResult response)
    {
        TextContent name = new()
        {
            Text = response.Name.Value,
            Link = new(url: $"/establishments/{response.UniqueIdentifier.Value}")
        };

        Dictionary<TextContent, TextContent> results = new()
        {
            {
                new TextContent(){ Text = "URN" },
                new TextContent(){ Text = response.UniqueIdentifier.Value }
            },
            {
                new TextContent(){ Text = "Type" },
                new TextContent(){ Text = response.Type!.Name }
            },
            {
                new TextContent(){ Text = "Address" },
                new TextContent(){ Text = response.Address!.FullAddress }
            },
            {
                new TextContent(){ Text = "Local authority" },
                new TextContent(){ Text = response.LocalAuthority!.Name }
            },
            {
                new TextContent(){ Text = "Part of a group" },
                new TextContent()
                {
                    Text = response.Group!.PartOfName,
                    Link = new(url: $"/groups/{response.Group?.PartOfCode}")
                }
            },
        };

        return (name, results);
    }

    private static (TextContent, IReadOnlyDictionary<TextContent, TextContent>) MapToGroupResultTable(SearchAggregateResult response)
    {
        TextContent name = new()
        {
            Text = response.Name.Value,
            Link = new(url: $"/establishments/{response.UniqueIdentifier.Value}")
        };

        Dictionary<TextContent, TextContent> results = new()
        {
            {
                new TextContent(){ Text = "Group ID" },
                new TextContent(){ Text = response.UniqueIdentifier.Value }
            },
            {
                new TextContent(){ Text = "Type" },
                new TextContent(){ Text = response.Type!.Name }
            },
            {
                new TextContent(){ Text = "Address" },
                new TextContent(){ Text = response.Address!.FullAddress }
            },
            {
                new TextContent(){ Text = "Academies" },
                new TextContent(){ Text = response.AcademyCount.ToString() }
            }
        };

        return (name, results);
    }
}
