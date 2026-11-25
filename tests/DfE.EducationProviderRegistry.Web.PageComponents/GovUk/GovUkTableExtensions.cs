using DfE.Document.Abstractions;

namespace DfE.EducationProviderRegistry.Web.PageComponents.GovUk;

public static class GovUkTableExtensions
{
    public static GovUkTable ToGovUkTable(this IElement element)
    {
        ArgumentNullException.ThrowIfNull(element);

        string? caption = element
            .Find("caption")?
            .Text
            .Trim();

        IReadOnlyList<GovUkTableRow> rows =
            element.FindAll("tbody tr")
                .Select(row =>
                {
                    IReadOnlyList<TextContent> cells =
                        row.FindAll("th, td")
                            .Select(cell =>
                            {
                                IElement? a = cell.Find("a");

                                return new TextContent
                                {
                                    Text = cell.Text.Trim(),
                                    Link = a is not null
                                        ? new Link(
                                            url: a.Attribute("href") ?? null,
                                            securityAttributes: a.Attribute("rel")?
                                                .Split(" ", StringSplitOptions.RemoveEmptyEntries) ?? [],
                                            opensInNewWindow: a.Attribute("target") == "_blank")
                                        : null
                                };
                            })
                            .ToArray();

                    return new GovUkTableRow
                    {
                        Cells = cells
                    };
                })
                .ToArray();

        return new GovUkTable
        {
            Caption = caption,
            Rows = rows
        };
    }
}
