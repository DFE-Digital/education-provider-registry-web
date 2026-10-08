using System.Text.Json;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Breadcrumbs.Models;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Breadcrumbs.Stores;

public sealed class SessionBreadcrumbJourneyStore : IBreadcrumbJourneyStore
{
    private const string SessionKey = "NavigationContext";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public SessionBreadcrumbJourneyStore(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public BreadcrumbJourney Get()
    {
        string? json =
            _httpContextAccessor
                .HttpContext?
                .Session
                .GetString(SessionKey);

        if (string.IsNullOrWhiteSpace(json))
        {
            return new BreadcrumbJourney();
        }

        return JsonSerializer.Deserialize<BreadcrumbJourney>(json)
            ?? new BreadcrumbJourney();
    }

    public void Save(BreadcrumbJourney journey)
    {
        string json = JsonSerializer.Serialize(journey);

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
