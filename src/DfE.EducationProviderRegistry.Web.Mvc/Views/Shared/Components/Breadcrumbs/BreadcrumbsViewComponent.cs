using DfE.EducationProviderRegistry.Web.Mvc.Features.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Breadcrumbs.Services;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Views.Shared.Components.Breadcrumbs;

public sealed class BreadcrumbsViewComponent : ViewComponent
{
    private readonly IBreadcrumbJourneyService _navigation;

    public BreadcrumbsViewComponent(IBreadcrumbJourneyService navigation)
    {
        ArgumentNullException.ThrowIfNull(navigation);
        _navigation = navigation;
    }

    public IViewComponentResult Invoke()
    {
        BreadcrumbJourney context = _navigation.Get();

        if (context.Items.Count <= 1)
            return Content(string.Empty);

        IReadOnlyCollection<BreadcrumbItem> breadcrumbs =
            context.Items
                .Take(context.Items.Count - 1)
                .ToList();

        if (breadcrumbs.Count is 0)
            return Content(string.Empty);

        return View(breadcrumbs);
    }
}
