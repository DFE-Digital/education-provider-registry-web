using Microsoft.AspNetCore.Http;

namespace DfE.EducationProviderRegistry.Web.SharedTests.TestDoubles;

public static class SessionTestDouble
{
    public static ISession Fake() => new FakeSession();
}
public sealed class FakeSession : ISession
{
    private readonly Dictionary<string, byte[]> _sessionStorage;

    public FakeSession()
    {
        _sessionStorage = new(StringComparer.OrdinalIgnoreCase);
    }

    public bool IsAvailable => true;

    public string Id => Guid.NewGuid().ToString();

    IEnumerable<string> Keys => _sessionStorage.Keys;

    IEnumerable<string> ISession.Keys => Keys;

    public void Clear() => _sessionStorage.Clear();

    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public void Remove(string key) => _sessionStorage.Remove(key);

    public void Set(string key, byte[] value) => _sessionStorage[key] = value;

    public bool TryGetValue(string key, out byte[] value) => _sessionStorage.TryGetValue(key, out value!);
}
