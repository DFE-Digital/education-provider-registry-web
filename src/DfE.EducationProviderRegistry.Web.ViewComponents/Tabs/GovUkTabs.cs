namespace DfE.EducationProviderRegistry.Web.ViewComponents.Tabs;

public sealed record GovUkTabs
{
    public GovUkTabs(IEnumerable<Tab> tabs)
    {
        ArgumentNullException.ThrowIfNull(tabs);

        if (!tabs.Any())
        {
            throw new ArgumentException("Tabs cannot be empty");
        }

        Tabs = tabs;
    }

    public IEnumerable<Tab> Tabs { get; init; }
}
