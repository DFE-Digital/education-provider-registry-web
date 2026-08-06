namespace DfE.WebDriver.WebDriver;

public static class WebDriverSessionRequestKeys
{
    public const string Headless = "headless";
    public const string Viewport = "viewport";
    public const string BrowserType = "browser";
    public const string BrowserVersion = "browserVersion";
    public const string AllowInsecureLocalhost = "allowInsecureLocalhost";
    public const string StartMaximised = "startMaximised";

    //public const string AcceptInsecureCertificates = "acceptInsecureCertificates";
}

internal sealed class WebDriverSessionContext
{
    private readonly IDictionary<string, object> _config;

    public WebDriverSessionContext() : this([])
    {

    }

    public WebDriverSessionContext(IEnumerable<KeyValuePair<string, object>> configuration)
    {
        _config =
            configuration?.ToDictionary(
                (k) => k.Key,
                (v) => v.Value) ?? [];
    }

    public bool Contains(string key)
    {
        return _config.ContainsKey(key);
    }

    public T GetRequired<T>(string key)
    {
        if (!_config.TryGetValue(key, out object? objValue))
        {
            throw new ArgumentException($"Required key {key} not found.");
        }

        if (objValue is not T typedValue)
        {
            throw new ArgumentException($"Required key {key} is not of type {typeof(T).Name}.");
        }

        return typedValue;
    }

    public bool TryGet<T>(string key, out T? value)
    {
        if (_config.TryGetValue(key, out object objValue) &&
            objValue is T typedValue)
        {
            value = typedValue;
            return true;
        }

        value = default!;

        return false;
    }

    public void Set(string key, object value)
    {
        _config[key] = value;
    }
}

internal sealed record WebDriverSessionRequest
{
    public WebDriverSessionRequest(
        WebDriverSessionContext request)
    {
        request.TryGet<bool>(WebDriverSessionRequestKeys.Headless, out bool headless);
        Headless = headless;

        request.TryGet<bool>(WebDriverSessionRequestKeys.StartMaximised, out bool maximised);
        StartMaximised = maximised;

        BrowserVersion =
            request.TryGet<string>(
                WebDriverSessionRequestKeys.BrowserVersion,
                out string? version)
                    ? version
                    : null;

        Viewport =
            request.TryGet<BrowserViewport>(
                WebDriverSessionRequestKeys.Viewport,
                out BrowserViewport viewport)
                    ? viewport
                    : null;
    }

    public bool Headless { get; }
    public bool StartMaximised { get; }
    public string? BrowserVersion { get; }
    public BrowserViewport? Viewport { get; }
}