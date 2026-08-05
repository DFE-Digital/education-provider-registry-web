using DfE.WebDriver.WebDriver;
using OpenQA.Selenium;

namespace DfE.WebDriver.Public.Session;

internal sealed class WebDriverSession : IWebDriverSession
{
    private readonly IWebDriverProviderRegistry _registry;
    private readonly WebDriverSessionRequest _request;

    public WebDriverSession(IWebDriverProviderRegistry registry, WebDriverSessionRequest spec)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _request = spec ?? throw new ArgumentNullException(nameof(spec));
    }

    public ValueTask<IWebDriver> StartDriverAsync(CancellationToken cancellationToken = default)
    {
        if (!_request.TryGet<string>(
                WebDriverSessionContextKeys.BrowserType,
                out string? browser))
        {
            throw new InvalidOperationException(
                "BrowserType was not specified.");
        }

        IWebDriverProvider provider = _registry.GetProvider(browser!);

        return new ValueTask<IWebDriver>(provider.GetDriver(_request));
    }
}
