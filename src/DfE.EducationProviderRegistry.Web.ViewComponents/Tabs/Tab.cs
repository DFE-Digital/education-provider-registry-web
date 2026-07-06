namespace DfE.EducationProviderRegistry.Web.ViewComponents.Tabs;

public sealed record Tab
{
    public Tab(
        string id,
        string title,
        IReadOnlyCollection<TabContent> contents)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentNullException.ThrowIfNull(contents);

        Id = id;
        Title = title;
        Contents = contents;
    }

    public string Id { get; }

    public string Title { get; }

    public IReadOnlyCollection<TabContent> Contents { get; }
}