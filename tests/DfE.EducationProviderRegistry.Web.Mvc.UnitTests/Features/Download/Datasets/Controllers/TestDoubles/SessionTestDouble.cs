using Microsoft.AspNetCore.Http;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Download.Datasets.Controllers.TestDoubles;

public class SessionTestDouble : ISession
{
    private readonly Dictionary<string, byte[]> _sessionStorage = new(StringComparer.OrdinalIgnoreCase);

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

    public static SessionTestDouble Create() => new();
}
