using AngleSharp.Html.Dom;
using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search.Configuration.Extensions;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using DfE.EducationProviderRegistry.Web.SharedTests.AngleSharp.Extensions;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.Search;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.Search.Components;
using Microsoft.Extensions.Configuration;

namespace DfE.EducationProviderRegistry.Web.Mvc.SystemTests.Search;

public sealed class SearchResultsTests : WebApplicationFactoryBaseSystemTest
{
    public SearchResultsTests(IServiceProvider provider) : base(provider)
    {
    }

    protected override void ConfigureApplicationConfiguration(IConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.AddSearchUseCaseConfiguration((builder) =>
        {
            builder
                .WithSearchTerm(
                    "what",
                    (field) =>
                        field.WithFieldName(nameof(SearchAggregate.ProviderName))
                            .AppendContainsMatchBehaviour())
                .WithSearchTerm(
                    "where",
                    (field) =>
                        field.WithFieldName(nameof(SearchAggregate.Postcode))
                            .AppendStartsWithMatchBehaviour());
        });
    }

    [Fact]
    public async Task Results_Returned_When_Search_By_What_Term()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        SearchAggregate match =
            SearchAggregateBuilder.Create()
                .WithProviderName("school 1")
                    .Build();

        SearchAggregate doesNotMatch =
            SearchAggregateBuilder.Create()
                .WithProviderName("College")
                    .Build();

        SearchAggregate[] seed = [match, doesNotMatch];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(seed, ct);

        using HttpClient client = Factory.CreateClient();

        using HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(Factory.Server.BaseAddress)
                .WithIdentitySearchTerm("sch")
                .Build();

        // Act
        using HttpResponseMessage response = await client.SendAsync(message, ct);

        // Assert
        using IHtmlDocument document = await response.AssertSuccessfulHtmlResponseAsync();

        SearchResultsComponent resultsComponent = new(document);
        SearchResult displayedResult = Assert.Single(resultsComponent.GetSearchResults());
        Assert.Equal(match.ProviderName, displayedResult.Name);
    }

    [Fact]
    public async Task Results_Returned_When_Search_By_Where_Term()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        SearchAggregate match1 =
            SearchAggregateBuilder.Create()
                .WithPostcode("BA22 6AS")
                    .Build();

        SearchAggregate match2 =
            SearchAggregateBuilder.Create()
                .WithPostcode("ba2 2aw")
                    .Build();

        SearchAggregate doesNotMatch =
            SearchAggregateBuilder.Create()
                .WithPostcode("B2 2aw")
                    .Build();

        SearchAggregate[] seed = [match1, match2, doesNotMatch];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(seed, ct);

        using HttpClient client = Factory.CreateClient();

        using HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(Factory.Server.BaseAddress)
                .WithLocationTerm("ba2")
                .Build();

        // Act
        using HttpResponseMessage response = await client.SendAsync(message, ct);

        // Assert
        using IHtmlDocument document = await response.AssertSuccessfulHtmlResponseAsync();

        SearchResultsComponent resultsComponent = new(document);
        IReadOnlyList<SearchResult> searchResults = resultsComponent.GetSearchResults();
        IReadOnlyList<string> searchResultNames = searchResults.Select(t => t.Name).ToList();

        Assert.Equal(2, searchResults.Count);
        Assert.Contains(match1.ProviderName, searchResultNames);
        Assert.Contains(match2.ProviderName, searchResultNames);
    }

    [Fact]
    public async Task Results_Returned_When_Search_By_Multiple_Terms()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        SearchAggregate match1 =
            SearchAggregateBuilder.Create()
                .WithProviderName("school 1")
                .WithPostcode("SW11 1EW")
                .Build();

        SearchAggregate match2 =
            SearchAggregateBuilder.Create()
                .WithProviderName("My School")
                .WithPostcode("SW11 5LP")
                .Build();

        SearchAggregate doesNotMatch =
            SearchAggregateBuilder.Create()
                .WithProviderName("College")
                .WithPostcode("SW11 5LP")
                .Build();

        SearchAggregate[] seed = [match1, match2, doesNotMatch];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(seed, ct);

        using HttpClient client = Factory.CreateClient();

        using HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(Factory.Server.BaseAddress)
                .WithIdentitySearchTerm("sch")
                .WithLocationTerm("SW11")
                .Build();

        // Act
        using HttpResponseMessage response = await client.SendAsync(message, ct);

        // Assert
        using IHtmlDocument document = await response.AssertSuccessfulHtmlResponseAsync();

        SearchResultsComponent resultsComponent = new(document);

        IReadOnlyList<SearchResult> searchResults = resultsComponent.GetSearchResults();
        IReadOnlyList<string> searchResultNames = searchResults.Select(t => t.Name).ToList();

        Assert.Equal(2, searchResults.Count);
        Assert.Contains(match1.ProviderName, searchResultNames);
        Assert.Contains(match2.ProviderName, searchResultNames);
    }

    [Fact]
    public async Task Results_Paged_When_Page_Requested()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        IReadOnlyCollection<SearchAggregate> seed = SearchAggregateTestDouble.CreateResults(count: 14, namePrefix: "school");

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(seed, ct);

        using HttpClient client = Factory.CreateClient();

        using HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(Factory.Server.BaseAddress)
                .WithIdentitySearchTerm("sch")
                .WithPage(2)
                .Build();

        // Act
        using HttpResponseMessage response = await client.SendAsync(message, ct);

        // Assert
        using IHtmlDocument document = await response.AssertSuccessfulHtmlResponseAsync();

        SearchResultsComponent resultsComponent = new(document);
        PaginationComponent component = new(document);

        IReadOnlyList<SearchResult> searchResults = resultsComponent.GetSearchResults();

        Assert.Equal(4, searchResults.Count);
        Assert.Equal("2", component.GetCurrentPage());
    }

    [Fact]
    public async Task Results_Sorted_When_Results_Sort_Applied()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        IReadOnlyCollection<SearchAggregate> seed = SearchAggregateTestDouble.CreateResults(count: 101, namePrefix: "school");

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(seed, ct);

        using HttpClient client = Factory.CreateClient();

        using HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(Factory.Server.BaseAddress)
                .WithIdentitySearchTerm("sch")
                .WithSortDirection("za")
                .Build();

        // Act
        using HttpResponseMessage response = await client.SendAsync(message, ct);

        // Assert
        using IHtmlDocument document = await response.AssertSuccessfulHtmlResponseAsync();

        SearchResultsComponent resultsComponent = new(document);
        IReadOnlyList<SearchResult> searchResults = resultsComponent.GetSearchResults();

        List<SearchAggregate> sortedSeedResults = [.. seed.OrderByDescending(t => t.ProviderName)];

        const int defaultPageSize = 10;

        Assert.Equal(
            sortedSeedResults.Select(t => t.ProviderName).Take(defaultPageSize),
            searchResults.Select(t => t.Name));
    }
}
