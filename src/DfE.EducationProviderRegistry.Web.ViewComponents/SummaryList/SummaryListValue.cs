namespace DfE.EducationProviderRegistry.Web.ViewComponents.SummaryList;

public sealed record SummaryListValue
{
    public SummaryListValue(
        string? text = null,
        string? href = null)
    {
        if (string.IsNullOrWhiteSpace(text) &&
            string.IsNullOrWhiteSpace(href))
        {
            throw new ArgumentException(
                "Either text or href must be provided.");
        }

        Text = text;
        Href = href;
    }

    public string? Text { get; }

    public string? Href { get; }
}