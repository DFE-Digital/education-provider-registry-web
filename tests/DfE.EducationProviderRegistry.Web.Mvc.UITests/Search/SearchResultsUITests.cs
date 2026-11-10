using DfE.Document.Abstractions;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using DfE.EducationProviderRegistry.Web.PageComponents;
using DfE.EducationProviderRegistry.Web.PageComponents.Pages;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.Search;
using DfE.EducationProviderRegistry.Web.SharedTests.PageComponents;

namespace DfE.EducationProviderRegistry.Web.MVC.UITests.Search;

public sealed class SearchResultsUITests : UIBaseTest
{
    public SearchResultsUITests(IServiceProvider provider) : base(provider)
    {
    }

    [Fact]
    public async Task Sort_Results_By_Name_Descending()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        IReadOnlyCollection<SearchAggregate> seed = SearchAggregateTestDouble.CreateResults(10);

        await HostedEnvironment.DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableSearchAggregates>(seed, ct);

        using IWebDriver driver = await WebDriverBuilder.Build().StartDriverAsync(ct);

        Uri uri = new(baseUri: HostedEnvironment.GetApplicationUrl(), relativeUri: SearchRoutes.SearchResults(identityTerm: "sch"));

        await driver.Navigate().GoToUrlAsync(uri);

        ICommandHandler commandDispatcher = new WebDriverCommandHandler(driver);
        IQueryHandler query = new WebDriverQueryHandler(driver);

        SearchPanelComponent panel = new(query, commandDispatcher);
        SearchResultsComponent results = new(query);

        SearchResult preSortFirstResult = results.GetSearchResults()[0];

        // Act
        panel.SortBy("name", SearchPanelComponent.SortDirection.Descending);

        // Assert
        SearchResult postSortFirstResult = results.GetSearchResults()[0];

        int comparison = string.Compare(
            preSortFirstResult.Name.Text, postSortFirstResult.Name.Text, StringComparison.Ordinal);

        Assert.True(comparison < 0, "Expected pre-sorted name to come before post-sort name when descending sort");
    }

    [Fact]
    public async Task Filter_Results_Applies_Filter()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        IReadOnlyCollection<SearchAggregate> seed = SearchAggregateTestDouble.CreateResults(10);

        await HostedEnvironment.DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableSearchAggregates>(seed, ct);

        using IWebDriver driver = await WebDriverBuilder.Build().StartDriverAsync(ct);

        Uri uri = new(baseUri: HostedEnvironment.GetApplicationUrl(), relativeUri: SearchRoutes.SearchResults(identityTerm: "sch"));

        await driver.Navigate().GoToUrlAsync(uri);

        IQueryHandler queryHandler = new WebDriverQueryHandler(driver);
        SearchResultsComponent results = new(queryHandler);
        SearchFiltersComponent filters = new(queryHandler, new WebDriverCommandHandler(driver));

        const string targetFacet = "Provider type";
        const string targetFacetValueLabel = "Academy converter";

        // Act
        filters.FilterBy(
            facetLabel: targetFacet,
            facetValueLabel: targetFacetValueLabel);

        // Assert
        IReadOnlyCollection<SearchResult> postFilterResults = results.GetSearchResults();

        // Selected results are filtered

        Assert.All(
            postFilterResults,
            (result) =>
            {
                (TextContent key, TextContent value) providerType = Assert.Single(
                        result.Values,
                        value => value.key.Text.Equals(
                        "Type",
                        StringComparison.OrdinalIgnoreCase));

                Assert.Equal(
                    targetFacetValueLabel,
                    providerType.value.Text,
                    ignoreCase: true);
            });

        // Assert selected filter is displayed
        SelectedFilter actualSingleSelectedFilter = Assert.Single(filters.GetSelectedFilters());
        Assert.StartsWith(targetFacetValueLabel, actualSingleSelectedFilter.Text);

        // Assert selected filter value is in correct form
        Assert.Equal("providertypename", actualSingleSelectedFilter.FilterName);
        Assert.Equal("Academy converter", actualSingleSelectedFilter.FilterId);
    }
}

