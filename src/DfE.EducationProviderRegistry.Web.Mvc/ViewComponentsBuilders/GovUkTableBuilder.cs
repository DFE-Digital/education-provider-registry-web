using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

namespace DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;

public class GovUkTableBuilder
{
    public static GovUkTableBuilder Create() => new();

    private string _caption = string.Empty;
    private string? _link;
    private bool _isCaptionLarge;
    private readonly List<TableColumn> _columns = [];
    private readonly List<TableRow> _rows = [];


    public GovUkTableBuilder WithCaption(string caption, string? link = null, bool large = false)
    {
        _caption = caption;
        _link = link;
        _isCaptionLarge = large;
        return this;
    }

    public GovUkTableBuilder WithColumns(params TableColumn[] columns)
    {
        _columns.Clear();
        _columns.AddRange(columns);

        return this;
    }

    public GovUkTableBuilder AddRow(params TableCell[] cells)
    {
        _rows.Add(new TableRow { Cells = cells.ToList() });
        return this;
    }

    public GovUkTable Build() => new(
        columns: _columns,
        rows: _rows,
        caption: _caption,
        captionLinkUrl: _link,
        captionSize: _isCaptionLarge ? TableCaptionSize.Large : TableCaptionSize.Medium
    );
}
