namespace DfE.EducationProviderRegistry.Web.Mvc.Middleware.Breadcrumbs.Options;

public sealed class BreadcrumbRouteRule
{
    public string PathPattern { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<BreadcrumbPrefixItem> BaseJourney { get; set; } = [];
}
