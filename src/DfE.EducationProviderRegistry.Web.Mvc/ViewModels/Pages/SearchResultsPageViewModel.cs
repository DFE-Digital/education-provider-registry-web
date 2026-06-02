using DfE.EducationProviderRegistry.Web.Mvc.ViewModels.Components;

namespace DfE.EducationProviderRegistry.Web.Mvc.ViewModels.Pages;

public class SearchResultsPageViewModel
{
    public string Query { get; set; }
    public List<EstablishmentSearchResultViewModel> Results { get; set; }
}
