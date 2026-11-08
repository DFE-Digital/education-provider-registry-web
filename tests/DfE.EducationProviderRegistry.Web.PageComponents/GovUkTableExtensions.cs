using DfE.Document.Abstractions;

namespace DfE.EducationProviderRegistry.Web.PageComponents;

public static class GovUkTableExtensions
{
    public static GovUkTable ToGovUkTable(this IElement element)
    {
        ArgumentNullException.ThrowIfNull(element);

        string? caption = element
            .FindAll("caption")
            .SingleOrDefault()?
            .Text
            .Trim();

        IReadOnlyDictionary<string, TextContent> rows =
            element.FindAll("tbody tr")
                 .ToDictionary(
                     (row) => row.Find("th")?.Text.Trim() ?? throw new ArgumentException("Could not find tbody > tr > th"),
                     (row) =>
                     {
                         IElement td = row.Find("td") ??
                            throw new ArgumentException("Could not find tbody > tr > td");

                         IElement? a = td.Find("a");

                         TextContent content = new()
                         {
                             Text = td.Text.Trim(),
                             Link = a is not null ?
                                new Link(
                                    url: a.Attribute("href") ?? null,
                                    securityAttributes: a.Attribute("rel")?.Split(" ", StringSplitOptions.RemoveEmptyEntries) ?? [],
                                    opensInNewWindow: a.Attribute("target") == "_blank")
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
