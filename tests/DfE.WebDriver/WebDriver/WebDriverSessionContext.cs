namespace DfE.WebDriver.WebDriver;

public static class WebDriverSessionRequestKeys
{
    public const string Headless = "headless";
    public const string BrowserType = "browser";
    public const string BrowserVersion = "browserVersion";
    public const string Viewport = "viewport";
    public const string StartMaximised = "startMaximised";
    public const string AllowInsecureCertificates = "allowInsecureCertificates";
    public const string AllowInsecureLocalhost = "allowInsecureLocalhost";

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