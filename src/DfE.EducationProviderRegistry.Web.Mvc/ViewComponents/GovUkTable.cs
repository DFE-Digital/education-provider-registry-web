namespace DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;

public class GovUkTable
{
    public string? Caption { get; set; }
    public bool IsCaptionLarge { get; set; } = false;
    public string? CaptionLinkUrl { get; set; }
    public List<string>? Headers { get; set; }
    public List<GovUkTableRow> Rows { get; set; } = new();
}
