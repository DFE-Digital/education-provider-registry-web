using Microsoft.AspNetCore.Http;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Extensions.TestDoubles;

public static class CookieFactoryTestDouble
{
    private const string CookieName = "cookies_policy";

    /// <summary>
    /// Creates a DefaultHttpContext with a test cookie collection containing
    /// the specified raw cookie value. If the value is null, the cookie is omitted.
    /// </summary>
    public static DefaultHttpContext CreateContextWithCookie(string rawCookieValue)
    {
        DefaultHttpContext context = new();

        IRequestCookieCollection cookies = CreateCookieCollection(rawCookieValue);
        context.Request.Cookies = cookies;

        return context;
    }

    /// <summary>
    /// Creates a test cookie collection containing the analytics cookie.
    /// </summary>
    public static IRequestCookieCollection CreateCookieCollection(string rawCookieValue)
    {
        RequestCookieCollection collection = [];

        if (!string.IsNullOrWhiteSpace(rawCookieValue))
        {
            collection.Add(CookieName, rawCookieValue);
        }

        return collection;
    }
}

public sealed class RequestCookieCollection : IRequestCookieCollection
{
    private readonly Dictionary<string, string> _cookies;

    public RequestCookieCollection()
    {
        _cookies = [];
    }

    public void Add(string key, string value)
    {
        _cookies[key] = value;
    }

    public string this[string key] =>
        _cookies.TryGetValue(
            key, out string? value) ? value : null!;

    public int Count => _cookies.Count;

    public ICollection<string> Keys => _cookies.Keys;

    public bool ContainsKey(string key) => _cookies.ContainsKey(key);

    public bool TryGetValue(string key, out string value) =>
        _cookies.TryGetValue(key, out value!);

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _cookies.GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _cookies.GetEnumerator();
}