using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;

namespace DfE.EducationProviderRegistry.Web.Mvc.ViewModels.Pages;

public class EstablishmentDetailsPageViewModel
{
    public string Heading { get; set; }

    public GovUkTable BasicDetails { get; set; }
    public GovUkTable Governors { get; set; }
    public GovUkTable History { get; set; }
}