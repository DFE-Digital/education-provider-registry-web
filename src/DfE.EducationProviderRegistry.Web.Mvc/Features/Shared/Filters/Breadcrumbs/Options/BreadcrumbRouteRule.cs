using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;

public sealed class BreadcrumbRouteRule
{
    public string PathPattern { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? TitleModelProperty { get; set; }
    public BreadcrumbJourneyResetMode JourneyResetMode { get; set; }
    public List<BreadcrumbPrefixItem> BaseJourney { get; set; } = [];
}
