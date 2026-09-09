using DfE.EducationProviderRegistry.Web.MVC.UITests.Components;
using DfE.EducationProviderRegistry.Web.MVC.UITests.Search;
using DfE.EducationProviderRegistry.Web.SharedTests.ApplicationContainer;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace DfE.EducationProviderRegistry.Web.MVC.UITests;

public sealed class AnalyticsUITests : UIBaseTest
{
    public AnalyticsUITests(IServiceProvider provider) : base(provider)
    {
    }

    [Fact]
    public async Task Reject_Analytics_Sets_Cookie_And_Does_Not_Send_Traffic_To_Clarity()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        using IWebDriver webDriver = await WebDriverBuilder.Build().StartDriverAsync(ct);
        UrlRequestsCounterNetworkHandler tracker = await RegisterNetworkMonitoringAsync(webDriver, ApplicationEnvironment);
        GovUkCookieBanner banner = new(webDriver);

        // Act
        banner.Reject();

        // Assert
        // Cookie preference is now set
        AssertPreferenceCookieSet(webDriver);
        await TriggerAnalyticsWithBrowserActionAsync(webDriver, ApplicationEnvironment);

        Assert.Equal(0, tracker.RequestsMatchCounter);
    }

    [Fact]
    public async Task Accept_Analytics_Starts_Sending_Traffic_To_Clarity()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        using IWebDriver webDriver = await WebDriverBuilder.Build().StartDriverAsync(ct);
        UrlRequestsCounterNetworkHandler tracker = await RegisterNetworkMonitoringAsync(webDriver, ApplicationEnvironment);
        GovUkCookieBanner banner = new(webDriver);

        // Act
        banner.Accept();

        // Assert
        AssertPreferenceCookieSet(webDriver);
        await TriggerAnalyticsWithBrowserActionAsync(webDriver, ApplicationEnvironment);

        Assert.True(tracker.RequestsMatchCounter > 0, "Clarity should be called when analytics events are triggered and user has agreed to analytics");
    }

    private static void AssertPreferenceCookieSet(IWebDriver driver)
    {
        const string AnalyticsCookieName = "cookies_policy";

        // Cookie preference is now set
        Cookie? cookie = driver.Manage().Cookies.GetCookieNamed(AnalyticsCookieName);
        Assert.NotNull(cookie);
    }

    private static async Task<UrlRequestsCounterNetworkHandler> RegisterNetworkMonitoringAsync(IWebDriver webDriver, ApplicationHostedEnvironment application)
    {
        const string ClarityDomain = "clarity.ms";

        await webDriver.Manage().Network.StartMonitoring();
        await webDriver.Navigate().GoToUrlAsync(application.GetApplicationUrl());

        UrlRequestsCounterNetworkHandler handler = new(ClarityDomain, transformer: UrlRequestsCounterNetworkHandler.RouteUrlToUnknownDomain);

        webDriver.Manage().Network.AddRequestHandler(handler);
        return handler;
    }

    private static Task TriggerAnalyticsWithBrowserActionAsync(IWebDriver webDriver, ApplicationHostedEnvironment application)
    {
        Uri uriTriggeringAnalytics = new(application.GetApplicationUrl(), SearchRoutes.Search());
        return webDriver.Navigate().GoToUrlAsync(uriTriggeringAnalytics);
    }
}

internal sealed class UrlRequestsCounterNetworkHandler : NetworkRequestHandler
{
    internal static HttpRequestData RouteUrlToUnknownDomain(HttpRequestData data)
    {
        // Sink off to invalid domain - browser should ignore or failed DNS
        // RFC 2606 reserves .invalid TLD
        // RFC 6761 documents how special-use domains should be treated
        data.Url = "somewhere.invalid";
        return data;
    }

    public UrlRequestsCounterNetworkHandler(string url, Func<HttpRequestData, HttpRequestData>? transformer = null)
    {
        RequestMatcher = (httpData) =>
        {
            bool match = httpData.Url?.Contains(url, StringComparison.OrdinalIgnoreCase) ?? false;
            if (match)
            {
                RequestsMatchCounter++;
            }
            return match;
        };

        RequestTransformer = transformer;

        // Empty

        RequestsMatchCounter = 0;
    }

    public int RequestsMatchCounter { get; private set; }
}

