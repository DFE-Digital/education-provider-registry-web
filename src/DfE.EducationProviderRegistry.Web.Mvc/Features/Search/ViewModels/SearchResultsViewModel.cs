using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

public class SearchResultsViewModel
{
    public required List<GovUkTable> EstablishmentResults { get; set; }
}