namespace DfE.EducationProviderRegistry.Web.ViewComponents.Table;

public enum TableCaptionSize
{
    Default,
    Small,
    Medium,
    Large,
    ExtraLarge
}

public static class GovUkCaptionSizeExtensions
{
    public static string ToCssClass(TableCaptionSize size)
    {
        return size switch
        {
            TableCaptionSize.Small => "govuk-table__caption--s",
            TableCaptionSize.Medium => "govuk-table__caption--m",
            TableCaptionSize.Large => "govuk-table__caption--l",
            TableCaptionSize.ExtraLarge => "govuk-table__caption--xl",
            _ => string.Empty
        };
    }
}
