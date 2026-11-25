using DfE.Document.Abstractions;
using DfE.EducationProviderRegistry.Web.PageComponents.GovUk;

namespace DfE.EducationProviderRegistry.Web.PageComponents.Pages;

public sealed class GroupPage
{
    private readonly ISearchContext _context;

    public GroupPage(ISearchContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public string? NoEstablishmentsMessage()
    {
        return _context
            .Find("#establishments .govuk-body")?.Text;
    }

    public IReadOnlyCollection<GroupEstablishment> Establishments()
    {
        GovUkTable? table =
            _context
                .Find("#establishments .govuk-table")?
                .ToGovUkTable();

        List<GroupEstablishment> establishments = [];

        foreach (GovUkTableRow row in table?.Rows ?? [])
        {
            TextContent name = row[0];
            TextContent urn = row[1];

            establishments.Add(
                new GroupEstablishment(
                    name.Text,
                    urn.Text,
                    name.Link
                        ?? throw new ArgumentException(
                            "Establishment name did not contain a link")));
        }

        return establishments;
    }
}


public sealed record GroupEstablishment
{
    public GroupEstablishment(string name, string urn, Link linkedTo)
    {
        Name = name;
        Urn = urn;
        Link = linkedTo;
    }

    public string Name { get; }
    public string Urn { get; }
    public Link Link { get; }
}
