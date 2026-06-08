using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;

public class GovUkTableViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(GovUkTable model)
    {
        return View(model);
    }
}


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

public class GovUkTable
{
    public string? Caption { get; set; }
    public bool IsCaptionLarge { get; set; } = false;
    public string? CaptionLinkUrl { get; set; }

    public List<string>? Headers { get; set; }

    public List<GovUkTableRow> Rows { get; set; } = new();
}

public class GovUkTableRow
{
    public List<GovUkTableCell> Cells { get; set; } = new();
}

public class GovUkTableCell
{
    public string Text { get; set; } = "";
    public string? LinkUrl { get; set; }
    public bool IsBold { get; set; } = false;
}