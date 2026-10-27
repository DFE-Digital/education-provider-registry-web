using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;

namespace DfE.EducationProviderRegistry.Web.SharedTests.TestDoubles.Filters;

public sealed class BreadcrumbJourneyBuilder
{
    private readonly BreadcrumbJourney _journey;

    public BreadcrumbJourneyBuilder()
    {
        _journey = new();
    }

    public static BreadcrumbJourneyBuilder Create() => new();

    public BreadcrumbJourneyBuilder WithItem(string title, string url)
    {
        _journey.Items.Add(
            new BreadcrumbItem(title, url));

        return this;
    }

    public BreadcrumbJourney Build()
    {
        return _journey;
    }
}
