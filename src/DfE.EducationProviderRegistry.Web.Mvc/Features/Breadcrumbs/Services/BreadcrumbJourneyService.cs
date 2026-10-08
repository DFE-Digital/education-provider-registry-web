using DfE.EducationProviderRegistry.Web.Mvc.Features.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Breadcrumbs.Stores;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Breadcrumbs.Services;

public sealed class BreadcrumbJourneyService : IBreadcrumbJourneyService
{
    private readonly IBreadcrumbJourneyStore _store;

    public BreadcrumbJourneyService(
        IBreadcrumbJourneyStore store)
    {
        _store = store;
    }

    public BreadcrumbJourney Get()
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
        BreadcrumbJourney context = new();

        context.Items.Add(
            new BreadcrumbItem(
                BreadcrumbItemType.Home,
                "Home",
                "/"));

        context.Items.Add(
            new BreadcrumbItem(
                BreadcrumbItemType.Search,
                "Search",
                "/search"));

        context.Items.Add(
            new BreadcrumbItem(
                BreadcrumbItemType.SearchResults,
                "Search Results",
                searchResultsUrl));

        _store.Save(context);
    }

    public void VisitEstablishment(
        string name,
        string url)
    {
        NavigateTo(
            new BreadcrumbItem(
                BreadcrumbItemType.Establishment,
                name,
                url));
    }

    public void VisitGroup(
        string name,
        string url)
    {
        NavigateTo(
            new BreadcrumbItem(
                BreadcrumbItemType.Group,
                name,
                url));
    }

    private void NavigateTo(
        BreadcrumbItem node)
    {
        BreadcrumbJourney context =
            _store.Get();

        //
        // Direct navigation.
        //
        if (!context.Items.Any())
        {
            return;
        }

        //
        // Refresh protection.
        //
        BreadcrumbItem? current =
            context.Items.LastOrDefault();

        if (current?.Url == node.Url)
        {
            return;
        }

        //
        // Already exists in trail.
        //
        int existingIndex =
            context.Items.FindIndex(
                x => x.Url.Equals(
                    node.Url,
                    StringComparison.OrdinalIgnoreCase));

        if (existingIndex >= 0)
        {
            context.Items =
                context.Items
                    .Take(existingIndex + 1)
                    .ToList();

            _store.Save(context);

            return;
        }

        context.Items.Add(node);

        _store.Save(context);
    }
}
