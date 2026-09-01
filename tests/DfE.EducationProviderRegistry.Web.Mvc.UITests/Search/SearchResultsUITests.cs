using DfE.EducationProviderRegistry.Web.SharedTests.ApplicationContainer;
using DfE.WebDriver.Public.Session;
using OpenQA.Selenium;
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
        CancellationToken ct = TestContext.Current.CancellationToken;

        using IWebDriver driver = await _webDriverSessionBuilder.Build().StartDriverAsync(ct);

        Uri uri = new(baseUri: _application.GetApplicationUrl(), relativeUri: "/search");

        await driver.Navigate().GoToUrlAsync(uri);

        SearchPanel panel = new(driver);
        SearchResults results = new(driver);

        panel.Search(("sch", string.Empty));

        SearchResult preSortFirstResult = results.GetSearchResults().First();

        panel.SortBy("name", SortDirection.Descending);

        SearchResult postSortFirstResult = results.GetSearchResults().First();

        int comparison = string.Compare(
            preSortFirstResult.Name, postSortFirstResult.Name, StringComparison.Ordinal);

        Assert.True(comparison < 0, "Expected pre-sorted name to come before post-sort name when descending sort");
    }
}