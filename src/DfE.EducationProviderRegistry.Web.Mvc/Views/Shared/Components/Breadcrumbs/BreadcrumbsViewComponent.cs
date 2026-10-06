using DfE.EducationProviderRegistry.Web.Mvc.Features.NavigationJourney;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Views.Shared.Components.Breadcrumbs;

public sealed class BreadcrumbsViewComponent : ViewComponent
{
    private readonly INavigationJourneyService _navigation;

    public BreadcrumbsViewComponent(
        INavigationJourneyService navigation)
    {
        _navigation = navigation;
    }

    public IViewComponentResult Invoke()
    {
        NavigationContext context =
            _navigation.Get();

        if (context.Journey.Count <= 1)
        {
            return Content(string.Empty);
        }

        IReadOnlyCollection<NavigationNode> breadcrumbs =
            context.Journey
                .Take(context.Journey.Count - 1)
                .ToList();

        if (!breadcrumbs.Any())
        {
            return Content(string.Empty);
        }

        return View(breadcrumbs);
    }
}
