namespace DfE.EducationProviderRegistry.Web.ViewComponents.Table;

public sealed class GovUkTable
{
    public IReadOnlyList<TableColumn> Columns { get; }
    public IReadOnlyList<IReadOnlyList<TableCell>> Rows { get; }
    public string? Caption { get; }
    public TableCaptionSize CaptionSize { get; }

    public GovUkTable(
        IReadOnlyList<TableColumn> columns,
        IReadOnlyList<IReadOnlyList<TableCell>> rows,
        string? caption = null,
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

            if (string.IsNullOrWhiteSpace(column.Text))
            {
                throw new ArgumentException("Column headers must not be empty.", nameof(columns));
            }

            if (column.IsRowHeader)
            {
                rowHeaderCount++;
            }
        }

        if (rowHeaderCount > 1)
        {
            throw new ArgumentException("Only one column can be marked as a row header.", nameof(columns));
        }

        foreach (IReadOnlyList<TableCell> row in rows)
        {
            ArgumentNullException.ThrowIfNull(row);

            if (row.Count != columns.Count)
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
    }
}
