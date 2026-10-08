namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Breadcrumbs.Models;

public sealed record BreadcrumbItem(
    BreadcrumbItemType Type,
    string Title,
    string Url);
