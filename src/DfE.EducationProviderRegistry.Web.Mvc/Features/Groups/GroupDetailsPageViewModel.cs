using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Groups;

public class GroupDetailsPageViewModel
{
    public string Heading { get; set; }
    public GovUkTable BasicDetailsTable { get; set; }
    public GovUkTable AcademiesTable { get; set; }
    public GovUkTable TrusteesTable { get; set; }
    public GovUkTable MembersTable { get; set; }
}