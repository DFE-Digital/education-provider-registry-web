namespace DfE.EducationProviderRegistry.Web.Mvc.ViewModels
{
    public sealed record Breadcrumb(
        string Text,
        string? Controller = null,
        string? Action = null);
}
