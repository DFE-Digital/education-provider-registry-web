using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Breadcrumbs.Models;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Views.Shared.Components.Breadcrumbs;

public sealed class BreadcrumbsViewComponent : ViewComponent
{
    private const string HttpContextItemKey = "CurrentBreadcrumbJourney";

    public IViewComponentResult Invoke()
    {
        if (!HttpContext.Items.TryGetValue(HttpContextItemKey, out object? obj) ||
            obj is not BreadcrumbJourney context ||
            context.Items.Count <= 1)
        {
            return Content(string.Empty);
        }

        IList<BreadcrumbItem> breadcrumbs =
            context.Items
                .Take(context.Items.Count - 1)
                .ToList();

        if (breadcrumbs.Count is 0)
            return Content(string.Empty);

        return View(breadcrumbs);
    }
}
