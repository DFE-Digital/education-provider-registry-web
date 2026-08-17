namespace DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;

public class GovUkTableBuilder
{
    private readonly GovUkTable _table = new();

    public static GovUkTableBuilder Create() => new();

    public GovUkTableBuilder WithCaption(string caption, string? link = null, bool large = false)
    {
        _table.Caption = caption;
        _table.CaptionLinkUrl = link;
        _table.IsCaptionLarge = large;
        return this;
    }

    public GovUkTableBuilder WithHeaders(params string[] headers)
    {
        _table.Headers = headers.ToList();
        return this;
    }

    public GovUkTableBuilder AddRow(params GovUkTableCell[] cells)
    {
        _table.Rows.Add(new GovUkTableRow { Cells = cells.ToList() });
        return this;
    }

    public GovUkTable Build() => _table;
}
