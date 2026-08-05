namespace DfE.WebDriver.WebDriver;

internal sealed class WebDriverProviderRegistry : IWebDriverProviderRegistry
{
    private readonly Dictionary<string, Func<IWebDriverProvider>> _webDriverProviders;

    public WebDriverProviderRegistry(Dictionary<string, Func<IWebDriverProvider>> webDriverProviders)
    {
        _webDriverProviders = webDriverProviders ?? [];
    }
    public IWebDriverProvider GetProvider(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("key cannot be null or whitespace.", nameof(key));
        }

        if (!_webDriverProviders.TryGetValue(key, out Func<IWebDriverProvider>? factory))
        {
            throw new InvalidOperationException($"WebDriverProvider is not registered with key {key}");
        }

        return factory();
    }
}
