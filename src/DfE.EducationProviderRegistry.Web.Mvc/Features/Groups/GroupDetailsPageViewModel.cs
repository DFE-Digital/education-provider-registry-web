using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Groups;

public sealed record GroupDetailsPageViewModel
{
    public required string Heading { get; set; }
    public required GroupDetailsTabViewModel Details { get; set; }
    public required GroupDetailsEstablishmentTabViewModel Establishments { get; init; }
    public required GroupDetailsGovernanceTabViewModel Governance { get; init; }
}

public sealed class GroupDetailsTabViewModel
{
    public string? Tab { get; set; }
    public GovUkTable? Table { get; set; }
}

public sealed class GroupDetailsEstablishmentTabViewModel
{
    public string? Tab { get; set; }
    public GovUkTable? Table { get; set; }
    public bool HasEstablishments => Table is not null;
    public string? NoEstablishmentsMessage { get; set; }
}

public sealed class GroupDetailsGovernanceTabViewModel
{
    public string? Tab { get; set; }
    public GovUkTable? TrusteesTable { get; set; }
    public GovUkTable? MembersTable { get; set; }
}
