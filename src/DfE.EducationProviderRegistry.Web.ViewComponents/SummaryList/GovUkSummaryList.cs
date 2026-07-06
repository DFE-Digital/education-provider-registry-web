namespace DfE.EducationProviderRegistry.Web.ViewComponents.SummaryList;

public sealed record GovUkSummaryList
{
    public GovUkSummaryList(
        IReadOnlyList<SummaryListRow> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        Rows = rows;
    }

    public IReadOnlyList<SummaryListRow> Rows { get; }
}