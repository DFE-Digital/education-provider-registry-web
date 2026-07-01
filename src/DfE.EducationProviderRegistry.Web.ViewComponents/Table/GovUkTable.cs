namespace DfE.EducationProviderRegistry.Web.ViewComponents.Table;

public sealed class GovUkTable
{
    public IReadOnlyList<GovUkTableColumn> Columns { get; }
    public IReadOnlyList<IReadOnlyList<GovUkTableCell>> Rows { get; }
    public string? Caption { get; }
    public GovUkCaptionSize CaptionSize { get; }

    public GovUkTable(
        IReadOnlyList<GovUkTableColumn> columns,
        IReadOnlyList<IReadOnlyList<GovUkTableCell>> rows,
        string? caption = null,
        GovUkCaptionSize captionSize = GovUkCaptionSize.Default)
    {
        ArgumentNullException.ThrowIfNull(columns);
        ArgumentNullException.ThrowIfNull(rows);

        if (columns.Count == 0)
        {
            throw new ArgumentException("At least one column must be defined.", nameof(columns));
        }

        int rowHeaderCount = 0;

        foreach (GovUkTableColumn column in columns)
        {
            ArgumentNullException.ThrowIfNull(column);

            if (string.IsNullOrWhiteSpace(column.Header))
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

        foreach (IReadOnlyList<GovUkTableCell> row in rows)
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

public class GovUkTableColumn
{
    public GovUkTableColumn(string header)
    {
        Header = header;
    }

    public string Header { get; }
    public bool IsRowHeader { get; init; }     // renders <th scope="row">
    public bool IsNumeric { get; init; }       // adds govuk-table__cell--numeric

    // Extensibility
    public string? Classes { get; init; }
}

public sealed record GovUkTableCell
{
    public string? Text { get; init; }
    public string? Href { get; init; }
}



public enum GovUkCaptionSize
{
    Default,
    Small,
    Medium,
    Large,
    ExtraLarge
}


public static class GovUkCaptionSizeExtensions
{
    public static string ToCssClass(GovUkCaptionSize size)
    {
        return size switch
        {
            GovUkCaptionSize.Small => "govuk-table__caption--s",
            GovUkCaptionSize.Medium => "govuk-table__caption--m",
            GovUkCaptionSize.Large => "govuk-table__caption--l",
            GovUkCaptionSize.ExtraLarge => "govuk-table__caption--xl",
            _ => string.Empty
        };
    }
}
