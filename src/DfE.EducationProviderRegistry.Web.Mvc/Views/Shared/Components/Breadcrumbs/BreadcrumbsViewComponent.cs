using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Store;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Views.Shared.Components.Breadcrumbs;

public sealed class BreadcrumbsViewComponent : ViewComponent
{
    private readonly IBreadcrumbJourneyStore _store;

    public BreadcrumbsViewComponent(IBreadcrumbJourneyStore store)
    {
        ArgumentNullException.ThrowIfNull(store);
        _store = store;
    }

    public IViewComponentResult Invoke()
    {
        BreadcrumbJourney journey = _store.Load();

        IReadOnlyCollection<BreadcrumbItem> breadcrumbs =
            journey.Items
                .SkipLast(1)
                .ToList();

        return View(breadcrumbs);
    }
}
