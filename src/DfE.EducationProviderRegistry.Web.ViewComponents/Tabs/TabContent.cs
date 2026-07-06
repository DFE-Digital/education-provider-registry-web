namespace DfE.EducationProviderRegistry.Web.ViewComponents.Tabs;

public sealed record TabContent
{
    public TabContent(
        string viewComponentName,
        object? model = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(viewComponentName);

        ViewComponentName = viewComponentName;
        Model = model;
    }

    public string ViewComponentName { get; }
    public object? Model { get; }
}