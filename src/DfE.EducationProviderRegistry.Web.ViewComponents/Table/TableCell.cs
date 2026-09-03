namespace DfE.EducationProviderRegistry.Web.ViewComponents.Table;

public sealed record TableCell
{
    public string? Text { get; init; }
    public string? Href { get; init; }
    public bool IsBold { get; set; }
    public bool OpenInNewTab { get; init; }

    /// <summary>
    /// An optional list of sub-rows to display under this row. This is used for rows that have multiple values..
    /// </summary>
    public IReadOnlyList<TableCellRow> Rows { get; init; } = [];
}