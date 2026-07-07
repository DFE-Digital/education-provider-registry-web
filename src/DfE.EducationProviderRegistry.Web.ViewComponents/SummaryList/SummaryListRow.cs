namespace DfE.EducationProviderRegistry.Web.ViewComponents.SummaryList;

public sealed record SummaryListRow
{
    public SummaryListRow(
        string key,
        SummaryListValue value,
        IReadOnlyCollection<SummaryListAction>? actions = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(value);

        Key = key;
        Value = value;
        Actions = actions ?? [];
    }

    public string Key { get; }

    public SummaryListValue Value { get; }

    public IReadOnlyCollection<SummaryListAction> Actions { get; }
}

public sealed record SummaryListAction
{
    public SummaryListAction(
        string text,
        string href)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        ArgumentException.ThrowIfNullOrWhiteSpace(href);

        Text = text;
        Href = href;
    }

    public string Text { get; }
    public string Href { get; }
}