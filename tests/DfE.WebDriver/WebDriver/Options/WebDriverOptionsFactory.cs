using OpenQA.Selenium;

namespace DfE.WebDriver.WebDriver.Options;

internal abstract class WebDriverOptionsFactory<TOptions> : IWebDriverOptionsFactory<TOptions> where TOptions : DriverOptions, new()
{
    private readonly IReadOnlyCollection<Action<WebDriverSessionRequest, TOptions>> _optionHandlers;

    protected WebDriverOptionsFactory(IEnumerable<KeyValuePair<string, Action<WebDriverSessionRequest, TOptions>>> handlers)
    {
        _optionHandlers =
        [
            .. DriverOptionsMappings.CreateSharedMappings<TOptions>().Select(x => x.Value),
            .. handlers.Select(x => x.Value)
        ];
    }

    protected virtual void ConfigureOptions(TOptions options) { }

    public TOptions CreateOptions(WebDriverSessionContext context)
    {
        TOptions options = new();
        ConfigureOptions(options);

        WebDriverSessionRequest request = new(context);

        foreach (Action<WebDriverSessionRequest, TOptions> handler in _optionHandlers)
        {
            handler.Invoke(request, options);
        }

        return options;
    }
}