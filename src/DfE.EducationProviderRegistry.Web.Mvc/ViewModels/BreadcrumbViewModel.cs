namespace DfE.EducationProviderRegistry.Web.Mvc.ViewModels
{
    public sealed record BreadcrumbViewModel(
        string Text,
        string? Controller = null,
        string? Action = null);
}
