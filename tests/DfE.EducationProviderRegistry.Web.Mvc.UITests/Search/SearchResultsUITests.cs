using OpenQA.Selenium;
using static DfE.EducationProviderRegistry.Web.MVC.UITests.Search.SearchPanelComponent;

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

        using IWebDriver driver = await WebDriverBuilder.Build().StartDriverAsync(ct);

        Uri uri = new(baseUri: ApplicationEnvironment.GetApplicationUrl(), relativeUri: SearchRoutes.SearchResults(identityTerm: "sch"));

        await driver.Navigate().GoToUrlAsync(uri);

        SearchPanelComponent panel = new(driver);
        SearchResultsComponent results = new(driver);

        SearchResult preSortFirstResult = results.GetSearchResults().First();

        // Act
        panel.SortBy("name", SortDirection.Descending);

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

        using IWebDriver driver = await WebDriverBuilder.Build().StartDriverAsync(ct);

        Uri uri = new(baseUri: ApplicationEnvironment.GetApplicationUrl(), relativeUri: SearchRoutes.SearchResults(identityTerm: "sch"));

        await driver.Navigate().GoToUrlAsync(uri);

        SearchResultsComponent results = new(driver);
        SearchFiltersComponent filters = new(driver);

        const string targetFacet = "Establishment Type";
        const string targetFacetValueLabel = "Primary School";

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

