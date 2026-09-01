
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments.ViewModels;

public class EstablishmentDetailsPageViewModel
{
    public string? Heading { get; set; }

    public GovUkTable? BasicDetails { get; set; }
    public GovUkTable? Governors { get; set; }
}