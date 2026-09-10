using DfE.EducationProviderRegistry.Web.MVC.UITests.Components;
using DfE.EducationProviderRegistry.Web.MVC.UITests.Search;
using DfE.EducationProviderRegistry.Web.SharedTests.ApplicationContainer;
using OpenQA.Selenium;

namespace DfE.EducationProviderRegistry.Web.MVC.UITests;

public sealed class AnalyticsUITests : UIBaseTest
{
    private readonly UrlRequestsCounterNetworkHandler clarityTracker;
    private readonly UrlRequestsCounterNetworkHandler tagManagerTracker;
    private readonly IReadOnlyList<NetworkRequestHandler> _requestHandlers;
    
    public AnalyticsUITests(IServiceProvider provider) : base(provider)
    {
        clarityTracker = new("clarity.ms", UrlRequestsCounterNetworkHandler.RouteUrlToUnknownDomain);
        tagManagerTracker = new("googletagmanager.com", UrlRequestsCounterNetworkHandler.RouteUrlToUnknownDomain);
        _requestHandlers = [clarityTracker, tagManagerTracker];
    }

    [Fact]
    public async Task Reject_Analytics_Sets_Cookie_And_Does_Not_Send_AnalyticsTraffic()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        using IWebDriver webDriver = await WebDriverBuilder.Build().StartDriverAsync(ct);

        await RegisterNetworkMonitoringAsync(webDriver, ApplicationEnvironment, _requestHandlers);
        CookieBanner banner = new(webDriver);

        // Act
        banner.Reject();

        // Assert
        // Cookie preference is now set
        AssertPreferenceCookieSet(webDriver);
        await TriggerAnalyticsWithBrowserActionAsync(webDriver, ApplicationEnvironment);

        Assert.Equal(0, clarityTracker.RequestsMatchCounter);
        Assert.Equal(0, tagManagerTracker.RequestsMatchCounter);
    }

    [Fact]
    public async Task Accept_Analytics_Starts_Sending_Traffic_To_Clarity()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        using IWebDriver webDriver = await WebDriverBuilder.Build().StartDriverAsync(ct);
        await RegisterNetworkMonitoringAsync(webDriver, ApplicationEnvironment, _requestHandlers);
        CookieBanner banner = new(webDriver);

        // Act
        banner.Accept();

        // Assert
        AssertPreferenceCookieSet(webDriver);
        await TriggerAnalyticsWithBrowserActionAsync(webDriver, ApplicationEnvironment);

        Assert.True(clarityTracker.RequestsMatchCounter > 0, "Clarity should be called when analytics events are triggered and user has agreed to analytics");
        Assert.True(tagManagerTracker.RequestsMatchCounter > 0, "TagManager should be called when analytics events are triggered and user has agreed to analytics");
    }

    private static void AssertPreferenceCookieSet(IWebDriver driver)
    {
        const string AnalyticsCookieName = "cookies_policy";

        // Cookie preference is now set
        Cookie? cookie = driver.Manage().Cookies.GetCookieNamed(AnalyticsCookieName);
        Assert.NotNull(cookie);
    }

    private static async Task RegisterNetworkMonitoringAsync(
        IWebDriver webDriver, 
        ApplicationHostedEnvironment application, 
        IEnumerable<NetworkRequestHandler> handlers)
    {
        

        await webDriver.Manage().Network.StartMonitoring();
        await webDriver.Navigate().GoToUrlAsync(application.GetApplicationUrl());

        foreach (var handler in handlers)
        {
            webDriver.Manage().Network.AddRequestHandler(handler);
        }
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

