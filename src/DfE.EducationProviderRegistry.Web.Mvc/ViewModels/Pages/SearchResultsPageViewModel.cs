using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;

namespace DfE.EducationProviderRegistry.Web.Mvc.ViewModels.Pages;

public class SearchResultsPageViewModel
{
    public string Query { get; set; }
    public List<GovUkTable> EstablishmentResults { get; set; }
}
