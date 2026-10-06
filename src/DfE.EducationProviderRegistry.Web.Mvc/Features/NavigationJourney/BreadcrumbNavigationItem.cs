using System.Text.Json;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.NavigationJourney;

// MODELS
public sealed class NavigationContext
{
    public List<NavigationNode> Journey { get; set; } = [];
}

public sealed record NavigationNode(
    NavigationNodeType Type,
    string Title,
    string Url);

public enum NavigationNodeType
{
    Home,
    Search,
    SearchResults,
    Establishment,
    Group
}


// INTERFACES
public interface INavigationContextStore
{
    NavigationContext Get();
    void Save(NavigationContext context);
    void Clear();
}

public sealed class SessionNavigationContextStore : INavigationContextStore
{
    private const string SessionKey = "NavigationContext";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public SessionNavigationContextStore(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public NavigationContext Get()
    {
        string? json =
            _httpContextAccessor
                .HttpContext?
                .Session
                .GetString(SessionKey);

        if (string.IsNullOrWhiteSpace(json))
        {
            return new NavigationContext();
        }

        return JsonSerializer.Deserialize<NavigationContext>(json)
            ?? new NavigationContext();
    }

    public void Save(NavigationContext context)
    {
        string json = JsonSerializer.Serialize(context);

        _httpContextAccessor
            .HttpContext!
            .Session
            .SetString(SessionKey, json);
    }

    public void Clear()
    {
        _httpContextAccessor
            .HttpContext!
            .Session
            .Remove(SessionKey);
    }
}

public interface INavigationJourneyService
{
    NavigationContext Get();

    void StartSearchJourney(string searchResultsUrl);

    void EnterEstablishment(string name, string url);

    void EnterGroup(string name, string url);

    void Clear();
}

public sealed class NavigationJourneyService
    : INavigationJourneyService
{
    private readonly INavigationContextStore _store;

    public NavigationJourneyService(
        INavigationContextStore store)
    {
        _store = store;
    }

    public NavigationContext Get()
    {
        return _store.Get();
    }

    public void Clear()
    {
        _store.Clear();
    }

    public void StartSearchJourney(
        string searchResultsUrl)
    {
        NavigationContext context = new();

        context.Journey.Add(
            new NavigationNode(
                NavigationNodeType.Home,
                "Home",
                "/"));

        context.Journey.Add(
            new NavigationNode(
                NavigationNodeType.Search,
                "Search",
                "/search"));

        context.Journey.Add(
            new NavigationNode(
                NavigationNodeType.SearchResults,
                "Search Results",
                searchResultsUrl));

        _store.Save(context);
    }

    public void EnterEstablishment(
        string name,
        string url)
    {
        NavigateTo(
            new NavigationNode(
                NavigationNodeType.Establishment,
                name,
                url));
    }

    public void EnterGroup(
        string name,
        string url)
    {
        NavigateTo(
            new NavigationNode(
                NavigationNodeType.Group,
                name,
                url));
    }

    private void NavigateTo(
        NavigationNode node)
    {
        NavigationContext context =
            _store.Get();

        //
        // Direct navigation.
        //
        if (!context.Journey.Any())
        {
            return;
        }

        //
        // Refresh protection.
        //
        NavigationNode? current =
            context.Journey.LastOrDefault();

        if (current?.Url == node.Url)
        {
            return;
        }

        //
        // Already exists in trail.
        //
        int existingIndex =
            context.Journey.FindIndex(
                x => x.Url.Equals(
                    node.Url,
                    StringComparison.OrdinalIgnoreCase));

        if (existingIndex >= 0)
        {
            context.Journey =
                context.Journey
                    .Take(existingIndex + 1)
                    .ToList();

            _store.Save(context);

            return;
        }

        context.Journey.Add(node);

        _store.Save(context);
    }
}
