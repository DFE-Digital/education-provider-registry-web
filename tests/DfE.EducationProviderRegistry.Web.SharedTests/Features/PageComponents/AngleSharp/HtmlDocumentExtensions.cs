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

        IReadOnlyDictionary<string, TextContent> rows =
            element.QuerySelectorAll("tbody tr")
                 .ToDictionary(
                     (row) => row.QuerySelector("th")?.Text().Trim() ?? throw new ArgumentException("Could not find tbody > tr > th"),
                     (row) =>
                     {
                         IElement td = row.QuerySelector("td") ??
                            throw new ArgumentException("Could not find tbody > tr > td");

                         IElement? a = td.QuerySelector("a");

                         TextContent content = new()
                         {
                             Text = td.Text().Trim(),
                             Link = a is not null ?
                                new Link(
                                    url: a.GetAttribute("href") ?? null,
                                    securityAttributes: a.GetAttribute("rel")?.Split(" ", StringSplitOptions.RemoveEmptyEntries) ?? [],
                                    opensInNewWindow: a.GetAttribute("target") == "_blank")
                                : null
                         };

                         return content;
                     });

        return new GovUkTable
        {
            Caption = caption,
            Rows = rows
        };
    }
}
