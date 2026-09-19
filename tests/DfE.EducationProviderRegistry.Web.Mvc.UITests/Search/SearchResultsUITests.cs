using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.Search;
using OpenQA.Selenium;

namespace DfE.EducationProviderRegistry.Web.MVC.UITests.Search;

public sealed class SearchResultsUITests : UIBaseTest
{
    public SearchResultsUITests(IServiceProvider provider) : base(provider)
    {
    }

    // TODO BiDI await network traffic that sort submitted
    [Fact]
    public async Task Sort_Results_By_Name_Descending()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        IReadOnlyCollection<SearchAggregate> seed = SearchAggregateTestDouble.CreateResults(10);

        await HostedEnvironment.DatabaseFixture
            .SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(seed, ct);

        using IWebDriver driver = await WebDriverBuilder.Build().StartDriverAsync(ct);

        Uri uri = new(baseUri: HostedEnvironment.GetApplicationUrl(), relativeUri: SearchRoutes.SearchResults(identityTerm: "sch"));

        await driver.Navigate().GoToUrlAsync(uri);

        SearchPanelComponent panel = new(driver);
        SearchResultsComponent results = new(driver);

        SearchResult preSortFirstResult = results.GetSearchResults().First();

        // Act
        panel.SortBy("name", SearchPanelComponent.SortDirection.Descending);

        // Assert
        SearchResult postSortFirstResult = results.GetSearchResults().First();

        int comparison = string.Compare(
            preSortFirstResult.Name, postSortFirstResult.Name, StringComparison.Ordinal);

        Assert.True(comparison < 0, "Expected pre-sorted name to come before post-sort name when descending sort");
    }

    [Fact]
    public async Task Filter_Results_Applies_Filter()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        IReadOnlyCollection<SearchAggregate> seed = SearchAggregateTestDouble.CreateResults(10);

        await HostedEnvironment.DatabaseFixture
            .SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(seed, ct);

        using IWebDriver driver = await WebDriverBuilder.Build().StartDriverAsync(ct);

        Uri uri = new(baseUri: HostedEnvironment.GetApplicationUrl(), relativeUri: SearchRoutes.SearchResults(identityTerm: "sch"));

        await driver.Navigate().GoToUrlAsync(uri);

        SearchResultsComponent results = new(driver);
        SearchFiltersComponent filters = new(driver);

        const string targetFacet = "searchprovidertypeid";
        const string targetFacetValueLabel = "Single-Academy Trust";

        // Act
        filters.FilterBy(
            facetLabel: targetFacet,
            facetValueLabel: targetFacetValueLabel);

        // Assert
        IReadOnlyCollection<SearchResult> postFilterResults = results.GetSearchResults();

        // Selected results are filtered
        Assert.All(
            postFilterResults,
            (result) => Assert.Equal(targetFacetValueLabel, result.Type, ignoreCase: true));

        // Assert selected filter is displayed
        SelectedFilter actualSingleSelectedFilter = Assert.Single(filters.GetSelectedFilters());
        Assert.StartsWith(targetFacetValueLabel, actualSingleSelectedFilter.Text);

        // Assert selected filter value is in correct form
        Assert.Equal(
            ConvertFacetSelectionToRemovalValue(filters, targetFacet, targetFacetValueLabel),
            actualSingleSelectedFilter.Value);
    }

    private static string ConvertFacetSelectionToRemovalValue(SearchFiltersComponent filters, string targetFacet, string targetFacetValueLabel)
    {
        string[] preselectionFilterValueParts =
            filters.GetFacetValueValue(
                targetFacet,
                targetFacetValueLabel)!.Split("-");

        return string.Concat(preselectionFilterValueParts[1], "|", preselectionFilterValueParts[2]);
    }
}

