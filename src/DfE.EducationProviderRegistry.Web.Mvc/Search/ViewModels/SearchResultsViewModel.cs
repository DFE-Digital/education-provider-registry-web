using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.ViewModels;

public class SearchResultsViewModel
{
    public required List<GovUkTable> EstablishmentResults { get; set; }
}