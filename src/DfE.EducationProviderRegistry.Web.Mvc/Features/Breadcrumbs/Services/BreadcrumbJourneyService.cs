using DfE.EducationProviderRegistry.Web.Mvc.Features.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Breadcrumbs.Stores;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Breadcrumbs.Services;

public sealed class BreadcrumbJourneyService : IBreadcrumbJourneyService
{
    private readonly IBreadcrumbJourneyStore _store;

    public BreadcrumbJourneyService(IBreadcrumbJourneyStore store)
    {
        ArgumentNullException.ThrowIfNull(store);
        _store = store;
    }

    public BreadcrumbJourney Get() => _store.Get();

    public void Clear() => _store.Clear();

    public void StartSearchJourney(string searchResultsUrl)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(searchResultsUrl);

        BreadcrumbJourney journey = new()
        {
            Items =
            [
                new BreadcrumbItem("Home", "/"),
                new BreadcrumbItem("Search", "/search"),
                new BreadcrumbItem("Search Results", searchResultsUrl)
            ]
        };

        _store.Save(journey);
    }

    public void NavigateTo(BreadcrumbDestination destination)
    {
        ArgumentNullException.ThrowIfNull(destination);

        BreadcrumbJourney journey = _store.Get();
        if (journey.Items.Count is 0)
            return;

        BreadcrumbItem? current = journey.Items.LastOrDefault();
        if (current?.Url.Equals(destination.Url, StringComparison.OrdinalIgnoreCase) is true)
            return;

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

            _store.Save(journey);

            return;
        }

        journey.Items.Add(
            new BreadcrumbItem(destination.Text, destination.Url));

        _store.Save(journey);
    }
}
