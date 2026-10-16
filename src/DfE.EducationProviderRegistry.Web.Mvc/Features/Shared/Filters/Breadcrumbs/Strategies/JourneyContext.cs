using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies;

public sealed record JourneyContext(
    BreadcrumbJourney Journey,
    BreadcrumbItem Page,
    string CurrentPath,
    string LastPath,
    int ExistingIndex
);
