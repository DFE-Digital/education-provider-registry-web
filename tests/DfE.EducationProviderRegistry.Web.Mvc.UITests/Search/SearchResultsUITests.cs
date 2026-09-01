using DfE.EducationProviderRegistry.Web.SharedTests.ApplicationContainer;
using DfE.WebDriver.Public.Session;
using OpenQA.Selenium;
using static DfE.EducationProviderRegistry.Web.MVC.UITests.Search.SearchFilters;
using static DfE.EducationProviderRegistry.Web.MVC.UITests.Search.SearchPanel;

namespace DfE.EducationProviderRegistry.Web.MVC.UITests.Search;

public sealed class SearchResultsUITests : IAsyncLifetime
{
    private readonly ApplicationHostedEnvironment _application;
    private readonly IWebDriverSessionBuilder _webDriverSessionBuilder;

    public SearchResultsUITests(
        ApplicationHostedEnvironment application,
        IWebDriverSessionBuilder webDriverSessionBuilder)
    {
        _application = application;
        _webDriverSessionBuilder = webDriverSessionBuilder;

        _webDriverSessionBuilder
            .WithChrome()
            .WithHeadless(true)
            .WithViewport(1920, 1080)
            .WithStartMaximised(true)
            .WithAllowInsecureLocalConnections(true)
            .Build();
    }

    public async ValueTask InitializeAsync()
    {
        await _application.InitialiseAsync(TestContext.Current.CancellationToken);
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    // TODO BiDI await network traffic that sort submitted
    [Fact]
    public async Task Sort_Results_By_Name_Descending()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        using IWebDriver driver = await _webDriverSessionBuilder.Build().StartDriverAsync(ct);

        Uri uri = GetSearchResultUriFor(_application, identityTerm: "sch");

        await driver.Navigate().GoToUrlAsync(uri);

        SearchPanel panel = new(driver);
        SearchResults results = new(driver);

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

        using IWebDriver driver = await _webDriverSessionBuilder.Build().StartDriverAsync(ct);

        Uri uri = GetSearchResultUriFor(_application, identityTerm: "sch");

        await driver.Navigate().GoToUrlAsync(uri);

        SearchResults results = new(driver);
        SearchFilters filters = new(driver);

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

    private static Uri GetSearchResultUriFor(
        ApplicationHostedEnvironment application,
        string? identityTerm = null,
        string? locationTerm = null,
        string? sort = null)
    {
        Uri baseUri = application.GetApplicationUrl();

        UriBuilder builder = new()
        {
            Scheme = baseUri.Scheme,
            Host = baseUri.Host,
            Port = baseUri.Port,
            Path = "/search/results",
            Query = $"?SearchKeywords={identityTerm ?? string.Empty}&Address={locationTerm ?? string.Empty}&sort={sort}"
        };

        return builder.Uri;
    }

    private static string ConvertFacetSelectionToRemovalValue(SearchFilters filters, string targetFacet, string targetFacetValueLabel)
    {
        string[] preselectionFilterValueParts =
            filters.GetFacetValueValue(
                targetFacet,
                targetFacetValueLabel)!.Split("-");

        return string.Concat(preselectionFilterValueParts[1], "|", preselectionFilterValueParts[2]);
    }
}

