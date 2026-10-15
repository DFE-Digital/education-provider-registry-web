using System.Text.Json;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Store;

public sealed class SessionBreadcrumbJourneyStore : IBreadcrumbJourneyStore
{
    private const string SessionKey = "BreadcrumbJourney";
    private readonly ISession _session;

    public SessionBreadcrumbJourneyStore(
        ISession session)
    {
        _session = session ??
            throw new ArgumentNullException(nameof(session));
    }

    public BreadcrumbJourney Load()
    {
        string? json = _session.GetString(SessionKey);

        return string.IsNullOrWhiteSpace(json)
            ? new BreadcrumbJourney()
            : JsonSerializer.Deserialize<BreadcrumbJourney>(json) ?? new BreadcrumbJourney();
    }

    public void Save(BreadcrumbJourney journey)
    {
        _session.SetString(SessionKey, JsonSerializer.Serialize(journey));
    }

    public void Clear()
    {
        _session.Remove(SessionKey);
    }
}
