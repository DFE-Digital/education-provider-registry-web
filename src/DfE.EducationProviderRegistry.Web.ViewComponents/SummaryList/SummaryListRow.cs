namespace DfE.EducationProviderRegistry.Web.ViewComponents.SummaryList;

public sealed record SummaryListRow
{
    public SummaryListRow(
        string key,
        SummaryListValue value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(value);

        Key = key;
        Value = value;
    }

    public string Key { get; }

    public SummaryListValue Value { get; }
}