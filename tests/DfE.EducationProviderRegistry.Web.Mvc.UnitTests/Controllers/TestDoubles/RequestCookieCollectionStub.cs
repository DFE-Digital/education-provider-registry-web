using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Controllers.TestDoubles;

public sealed class RequestCookieCollectionStub : IRequestCookieCollection
{
    private readonly Dictionary<string, string> _cookies = [];

    public string? this[string key] =>
        _cookies.TryGetValue(key, out string? value) ? value : null;

    public int Count => _cookies.Count;

    public ICollection<string> Keys => _cookies.Keys;

    public void Add(string key, string value) => _cookies[key] = value;

    public bool ContainsKey(string key) => _cookies.ContainsKey(key);

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() =>
        _cookies.GetEnumerator();

    public bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
    {
        if (_cookies.TryGetValue(key, out string? temp))
        {
            value = temp;
            return true;
        }

        value = null;
        return false;
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() =>
        _cookies.GetEnumerator();

}