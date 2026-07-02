namespace DfE.EducationProviderRegistry.Web.ViewComponents.Tabs;

public sealed record Tab
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public required IReadOnlyCollection<TabContent> Contents { get; init; }
}