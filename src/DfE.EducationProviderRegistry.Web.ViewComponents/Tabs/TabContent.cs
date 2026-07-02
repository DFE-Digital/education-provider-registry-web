namespace DfE.EducationProviderRegistry.Web.ViewComponents.Tabs;

public sealed record TabContent
{
    public required string ViewComponentName { get; init; }
    public object? Model { get; init; }
}
