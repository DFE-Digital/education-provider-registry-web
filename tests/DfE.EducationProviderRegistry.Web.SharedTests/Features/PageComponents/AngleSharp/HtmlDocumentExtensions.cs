using AngleSharp.Dom;

namespace DfE.EducationProviderRegistry.Web.SharedTests.Features.PageComponents.AngleSharp;

internal static class HtmlDocumentExtensions
{
    internal static GovUkTable ToGovUkTable(this IElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        string? caption = element
            .QuerySelectorAll("caption")
            .SingleOrDefault()?
            .Text()
            .Trim();

        IReadOnlyDictionary<string, string> rows =
            element.QuerySelectorAll("tbody tr")
                 .ToDictionary(
                     (row) => row.QuerySelector("th")?.Text().Trim() ?? throw new ArgumentException("Could not find tbody > tr > th"),
                     (row) => row.QuerySelector("td")?.Text().Trim() ?? throw new ArgumentException("Could not find tbody > tr > td"));

        return new GovUkTable
        {
            Caption = caption,
            Rows = rows
        };
    }
}
