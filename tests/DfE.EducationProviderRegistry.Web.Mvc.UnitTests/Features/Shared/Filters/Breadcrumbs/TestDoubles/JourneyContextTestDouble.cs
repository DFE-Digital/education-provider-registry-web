using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Filters.Breadcrumbs.TestDoubles;

public static class JourneyContextTestDouble
{
    public static JourneyContext CreateEmpty() =>
        Create();

    public static JourneyContext CreateWithJourney(
        params BreadcrumbItem[] items)
    {
        BreadcrumbJourney journey = new();

        journey.Items.AddRange(items);

        return Create(journey: journey);
    }

    public static JourneyContext CreateWithPageAndJourney(
        BreadcrumbItem page,
        params BreadcrumbItem[] journeyItems)
    {
        BreadcrumbJourney journey = new();
        journey.Items.AddRange(journeyItems);

        return Create(
            journey: journey,
            page: page);
    }

    public static JourneyContext Create(
        BreadcrumbJourney? journey = null,
        BreadcrumbItem? page = null,
        string? currentPath = null,
        string? lastPath = null,
        int existingIndex = -1)
    {
        page ??= new BreadcrumbItem(
            "Test Page",
            "/test");

        currentPath ??= "/test";
        lastPath ??= string.Empty;

        return new JourneyContext(
            journey ?? new BreadcrumbJourney(),
            page,
            currentPath,
            lastPath,
            existingIndex);
    }
}
