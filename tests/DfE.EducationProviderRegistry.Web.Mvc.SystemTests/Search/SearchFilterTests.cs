using AngleSharp.Html.Dom;
using DfE.Document.Abstractions;
using DfE.Document.Anglesharp;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using DfE.EducationProviderRegistry.Web.PageComponents.Pages;
using DfE.EducationProviderRegistry.Web.SharedTests.AngleSharp.Extensions;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.Search;
using SearchFiltersComponent = DfE.EducationProviderRegistry.Web.PageComponents.Pages.SearchFiltersComponent;
using SearchResultsComponent = DfE.EducationProviderRegistry.Web.PageComponents.Pages.SearchResultsComponent;

namespace DfE.EducationProviderRegistry.Web.Mvc.SystemTests.Search;

public sealed class SearchFilterTests : WebApplicationFactoryBaseSystemTest
{
    public SearchFilterTests(IServiceProvider provider) : base(provider)
    {
    }

    [Fact]
    public async Task Apply_A_Filter_Returns_Filtered_Results()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableSearchAggregates>(
            SearchAggregateTestDouble.CreateResults(10), ct);

        using HttpClient client = Factory.CreateClient();

        string[] filtersToApply = ["Academy converter"];

        HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(Factory.Server.BaseAddress)
                .WithIdentitySearchTerm("sch")
                .WithFilter("providertypename", filtersToApply)
                .Build();

        // Act
        HttpResponseMessage response = await client.SendAsync(message, ct);

        // Assert
        ISearchContext context =
                SearchContext.From(document: await response.AssertSuccessfulHtmlResponseAsync());

        SearchFiltersComponent filters = new(context);
        SearchResultsComponent results = new(context);

        Assert.NotEmpty(results.GetSearchResults());

        Filter selectedFilter = filters.GetFilters().Single(t => t.Name == "Provider type");

        // Assert FilterValue
        FilterValue value = Assert.Single(selectedFilter.FilterValues);

        Assert.True(value.Selected);
        Assert.NotEmpty(value.Label);
        Assert.Equal(filtersToApply.Single(), value.Value);
    }

    [Fact]
    public async Task Apply_A_Filter_With_Multiple_FilterValues_Returns_Filtered_Results()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableSearchAggregates>(
            SearchAggregateTestDouble.CreateResults(10), ct);

        using HttpClient client = Factory.CreateClient();

        string[] filtersToApply = ["Academy converter", "Community school"];

        HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(Factory.Server.BaseAddress)
                .WithIdentitySearchTerm("sch")
                .WithFilter("providertypename", filtersToApply)
                .Build();
        // Act
        HttpResponseMessage response = await client.SendAsync(message, ct);

        // Assert
        ISearchContext context =
                SearchContext.From(document: await response.AssertSuccessfulHtmlResponseAsync());
        SearchFiltersComponent filters = new(context);
        SearchResultsComponent results = new(context);

        Assert.NotEmpty(results.GetSearchResults());

        // Assert Filter
        Filter selectedFilter = filters.GetFilters().Single(t => t.Name == "Provider type");

        // Assert FilterValues
        Assert.Equal(2, selectedFilter.FilterValues.Count);

        FilterValue value1 = selectedFilter.FilterValues.Single(v => v.Value == filtersToApply[0]);
        Assert.True(value1.Selected);
        Assert.NotEmpty(value1.Label);

        FilterValue value2 = selectedFilter.FilterValues.Single(v => v.Value == filtersToApply[1]);
        Assert.True(value2.Selected);
        Assert.NotEmpty(value2.Label);
    }

    [Fact]
    public async Task Apply_Multiple_Filters_Returns_Filtered_Results()
    {
        Assert.Skip("Multiple filters not yet available");
    }


    [Fact]
    public async Task Remove_A_Filter_Returns_Remaining_Filtered_Results()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableSearchAggregates>(
            SearchAggregateTestDouble.CreateResults(10), ct);

        using HttpClient client = Factory.CreateClient();

        string[] filtersToApply = ["Academy converter", "Community school"];

        HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(Factory.Server.BaseAddress)
                .WithIdentitySearchTerm("sch")
                .WithFilter("providertypename", filtersToApply)
                .Build();

        HttpResponseMessage filteredResults = await client.SendAsync(message, ct);

        IHtmlDocument document = await filteredResults.AssertSuccessfulHtmlResponseAsync();

        SearchFiltersComponent filtersApplied =
            new(SearchContext.From(document));

        // Act
        HttpResponseMessage removalResponse =
            await filtersApplied.RemoveFilterAsync(
                client,
                new Uri(document.DocumentUri),
                facet: "providertypename",
                facetValue: "Community school",
                ct);

        // Assert
        ISearchContext context =
            SearchContext.From(document: await removalResponse.AssertSuccessfulHtmlResponseAsync());

        SearchFiltersComponent removedFilters = new(context);
        SearchResultsComponent results = new(context);

        Assert.NotEmpty(results.GetSearchResults());

        Filter remainingSelectedFilter = removedFilters.GetFilters().Single(t => t.Name == "Provider type");

        FilterValue remainingSelectedFilterValue = remainingSelectedFilter.FilterValues.Single();
        Assert.True(remainingSelectedFilterValue.Selected);
        Assert.Equal("Academy converter", remainingSelectedFilterValue.Value);
        Assert.NotEmpty(remainingSelectedFilterValue.Label);
    }

    [Fact]
    public async Task Clear_Filters_Removes_Applied_Filters()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;
        // TODO stub UseCaseResponse with results
        using HttpClient client = Factory.CreateClient();

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableSearchAggregates>(
            SearchAggregateTestDouble.CreateResults(10), ct);

        string[] filtersToApply = ["Academy converter", "Community school"];

        HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(Factory.Server.BaseAddress)
                .WithIdentitySearchTerm("sch")
                .WithFilter("providertypename", filtersToApply)
                .Build();

        HttpResponseMessage filteredResults = await client.SendAsync(message, ct);

        IHtmlDocument document = await filteredResults.AssertSuccessfulHtmlResponseAsync();

        SearchFiltersComponent filtersApplied =
            new(SearchContext.From(document));

        // Act
        HttpResponseMessage removalResponse =
            await filtersApplied.ClearFiltersAsync(client, new Uri(document.DocumentUri), ct);

        // Assert
        ISearchContext context =
            SearchContext.From(document: await removalResponse.AssertSuccessfulHtmlResponseAsync());

        SearchFiltersComponent clearedFilters = new(context);
        SearchResultsComponent results = new(context);

        Assert.NotEmpty(results.GetSearchResults());

        Filter filters = clearedFilters.GetFilters().Single(t => t.Name == "Provider type");

        // static 2 filter values in data. None selected
        FilterValue value1 = filters.FilterValues.Single(v => v.Value == filtersToApply[0]);
        Assert.False(value1.Selected);
        Assert.NotEmpty(value1.Label);

        FilterValue value2 = filters.FilterValues.Single(v => v.Value == filtersToApply[1]);
        Assert.False(value2.Selected);
        Assert.NotEmpty(value2.Label);
    }
}
