using BrowserLibrary.WebDriver;

namespace BrowserLibrary.Public.Session;

internal sealed class WebDriverSessionBuilder : IWebDriverSessionBuilder
{
    private readonly IWebDriverProviderRegistry _providerRegistry;
    private bool _startMaximised = true;
    private bool _headless = false;
    private bool _insecureLocal = false;
    private string _browserType = "chrome";
    private string? _browserVersion = null;
    private (int width, int height)? _viewportCartesian = null;

    public WebDriverSessionBuilder(IWebDriverProviderRegistry providerRegistry)
    {
        _providerRegistry = providerRegistry ??
            throw new ArgumentNullException(nameof(providerRegistry));
    }

    public IWebDriverSession Build()
    {
        WebDriverSessionRequest req = new();

        req.Set(WebDriverSessionContextKeys.BrowserType, _browserType);
        req.Set(WebDriverSessionContextKeys.Headless, _headless);
        req.Set(WebDriverSessionContextKeys.StartMaximised, _startMaximised);
        req.Set(WebDriverSessionContextKeys.AllowInsecureLocalhost, _insecureLocal);

        if (!string.IsNullOrWhiteSpace(_browserVersion))
        {
            req.Set(WebDriverSessionContextKeys.BrowserVersion, _browserVersion);
        }

        if (_viewportCartesian != null)
        {
            req.Set(
                WebDriverSessionContextKeys.Viewport,
                    new BrowserViewport(_viewportCartesian.Value.width, _viewportCartesian.Value.height));
        }

        return new WebDriverSession(_providerRegistry, req);
    }

    public IWebDriverSessionBuilder WithChrome() => WithBrowser(type: "chrome");

    public IWebDriverSessionBuilder WithEdge() => WithBrowser(type: "edge");

    public IWebDriverSessionBuilder WithFirefox() => WithBrowser(type: "firefox");

    public IWebDriverSessionBuilder WithBrowser(string type)
    {
        _browserType = type;
        return this;
    }

    public IWebDriverSessionBuilder WithBrowserVersion(string version)
    {
        _browserVersion = version;
        return this;
    }

    public IWebDriverSessionBuilder WithHeadless(bool headless)
    {
        _headless = headless;
        return this;
    }

    public IWebDriverSessionBuilder WithStartMaximised(bool maximised)
    {
        _startMaximised = maximised;
        return this;
    }

    public IWebDriverSessionBuilder WithAllowInsecureLocalConnections(bool insecure)
    {
        _insecureLocal = insecure;
        return this;
    }

    public IWebDriverSessionBuilder WithViewport(int width, int height)
    {
        _viewportCartesian = (width, height);
        return this;
    }
}