using System.Text.Json;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Breadcrumbs.Models;
using Microsoft.AspNetCore.Http;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Breadcrumbs.Services;

public sealed class SessionBreadcrumbJourneyService : IBreadcrumbJourneyService
{
    private const string SessionKey = "BreadcrumbJourney";
    private readonly ISession _session;

    public SessionBreadcrumbJourneyService(ISession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        _session = session;
    }

    public BreadcrumbJourney Get()
    {
        string? json = _session.GetString(SessionKey);
        if (string.IsNullOrWhiteSpace(json))
            return new BreadcrumbJourney();

        return JsonSerializer.Deserialize<BreadcrumbJourney>(json)
            ?? new BreadcrumbJourney();
    }

    public void Clear() => _session.Remove(SessionKey);

    public void StartSearchJourney(string searchResultsUrl)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(searchResultsUrl);

        BreadcrumbJourney journey = new()
        {
            Items =
                [
                    new BreadcrumbItem("Home","/"),
                    new BreadcrumbItem("Search", "/search"),
                    new BreadcrumbItem("Search Results", searchResultsUrl)
                ]
        };

        Save(journey);
    }

    public void Visit(BreadcrumbItem destination)
    {
        ArgumentNullException.ThrowIfNull(destination);
        BreadcrumbJourney journey = Get();

        //
        // Direct navigation.
        //
        if (journey.Items.Count is 0)
        {
            journey.Items.Add(destination);
            Save(journey);

            return;
        }

        //
        // Refresh detection.
        //
        BreadcrumbItem current = journey.Items.Last();

        if (current.Url.Equals(destination.Url, StringComparison.OrdinalIgnoreCase))
            return;

        //
        // Existing page in journey.
        //
        int existingIndex =
            journey.Items.FindIndex(
                x => x.Url.Equals(
                    destination.Url,
                    StringComparison.OrdinalIgnoreCase));

        if (existingIndex >= 0)
        {
            journey.Items =
                journey.Items
                    .Take(existingIndex + 1)
                    .ToList();

            Save(journey);

            return;
        }

        //
        // New page.
        //
        journey.Items.Add(destination);

        Save(journey);
    }

    private void Save(BreadcrumbJourney journey)
    {
        string json = JsonSerializer.Serialize(journey);

        _session.SetString(SessionKey, json);
    }
}
