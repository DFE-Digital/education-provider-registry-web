namespace DfE.EducationProviderRegistry.Web.ViewComponents.Table;

public sealed class GovUkTable
{
    public IReadOnlyList<TableColumn> Columns { get; }
    public IReadOnlyList<TableRow> Rows { get; }
    public string? Caption { get; }
    public TableCaptionSize CaptionSize { get; }
    public string? CaptionLinkUrl { get; }

    public GovUkTable(
        IReadOnlyList<TableColumn> columns,
        IReadOnlyList<TableRow> rows,
        string? caption = null,
        string? captionLinkUrl = null,
        TableCaptionSize captionSize = TableCaptionSize.Default)
    {
        ArgumentNullException.ThrowIfNull(columns);
        ArgumentNullException.ThrowIfNull(rows);

        if (columns.Count == 0)
        {
            throw new ArgumentException("At least one column must be defined.", nameof(columns));
        }

        int rowHeaderCount = 0;

        foreach (TableColumn column in columns)
        {
            ArgumentNullException.ThrowIfNull(column);

            if (column.IsRowHeader)
            {
                rowHeaderCount++;
            }
        }

        if (rowHeaderCount > 1)
        {
            throw new ArgumentException("Only one column can be marked as a row header.", nameof(columns));
        }

        foreach (TableRow row in rows)
        {
            ArgumentNullException.ThrowIfNull(row);

            if (row.Cells.Count != columns.Count)
            {
                throw new ArgumentException(
                    "Each row must have the same number of values as columns.",
                    nameof(rows));
            }
        }

        Columns = columns;
        Rows = rows;
        Caption = caption;
        CaptionSize = captionSize;
        CaptionLinkUrl = captionLinkUrl;
    }
}
