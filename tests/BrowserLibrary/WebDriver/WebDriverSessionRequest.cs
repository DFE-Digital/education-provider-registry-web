namespace BrowserLibrary.WebDriver;

public static class WebDriverSessionContextKeys
{
    public const string Headless = "headless";
    public const string Viewport = "viewport";
    public const string BrowserType = "browser";
    public const string BrowserVersion = "browserVersion";
    public const string AllowInsecureLocalhost = "allowInsecureLocalhost";
    public const string StartMaximised = "startMaximised";

    //public const string AcceptInsecureCertificates = "acceptInsecureCertificates";
}

internal sealed class WebDriverSessionRequest
{
    private readonly IDictionary<string, object> _config;

    public WebDriverSessionRequest() : this([])
    {

    }

    public WebDriverSessionRequest(IEnumerable<KeyValuePair<string, object>> configuration)
    {
        _config =
            configuration?.ToDictionary(
                (k) => k.Key,
                (v) => v.Value) ?? [];
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