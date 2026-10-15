using System.Text.Json;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Views.Shared.Components.Breadcrumbs;

public sealed class BreadcrumbsViewComponent : ViewComponent
{
    private const string SessionKey = "BreadcrumbJourney";
    private readonly ISession _session;

    public BreadcrumbsViewComponent(ISession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        _session = session;
    }


    public IViewComponentResult Invoke()
    {
        string? json = _session.GetString(SessionKey);

        BreadcrumbJourney journey =
            string.IsNullOrWhiteSpace(json)
                ? new BreadcrumbJourney()
                : JsonSerializer.Deserialize<BreadcrumbJourney>(json)
                  ?? new BreadcrumbJourney();

        IReadOnlyCollection<BreadcrumbItem> breadcrumbs =
            journey.Items
                .SkipLast(1)
                .ToList();

        return View(breadcrumbs);
    }
}
