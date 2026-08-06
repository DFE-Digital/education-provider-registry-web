using OpenQA.Selenium;

namespace DfE.WebDriver.WebDriver.Options;

internal class WebDriverOptionsFactory<TOptions> where TOptions : DriverOptions, new()
{
    private readonly IReadOnlyDictionary<string, Action<WebDriverSessionRequest, TOptions>> _handlerMap;

    protected WebDriverOptionsFactory(IEnumerable<KeyValuePair<string, Action<WebDriverSessionRequest, TOptions>>> handlers)
    {
        _handlerMap = 
            handlers?.ToDictionary(
                t => t.Key, 
                t => t.Value) ?? [];
    }

    protected virtual void ConfigureOptions(TOptions options) { }

    public TOptions CreateOptions(WebDriverSessionContext spec)
    {
        TOptions options = new();
        ConfigureOptions(options);

        WebDriverSessionRequest request = new(spec);

        foreach (Action<WebDriverSessionRequest, TOptions> handler in _handlerMap
                .Select((handlerContained) => handlerContained.Value))
        {
            handler.Invoke(request, options);
        }

        return options;
    }
}