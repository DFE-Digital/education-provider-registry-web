namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Groups;

public sealed record GroupDetailsPageViewModel
{
    public required string Heading { get; set; }
    public required GroupDetailsTabViewModel Details { get; set; }
    public required GroupDetailsAcademyTabViewModel Academies { get; init; }
    public required GroupDetailsGovernanceTabViewModel Governance { get; init; }
}

public sealed class GroupDetailsTabViewModel
{
    public string Tab { get; set; }
    public Web.ViewComponents.Table.GovUkTable Details { get; set; }
}

public sealed class GroupDetailsAcademyTabViewModel
{
    public string Tab { get; set; }
    public Web.ViewComponents.Table.GovUkTable Academies { get; set; }
}

public sealed class GroupDetailsGovernanceTabViewModel
{
    public string Tab { get; set; }
    public Web.ViewComponents.Table.GovUkTable Trustees { get; set; }
    public Web.ViewComponents.Table.GovUkTable Members { get; set; }
}