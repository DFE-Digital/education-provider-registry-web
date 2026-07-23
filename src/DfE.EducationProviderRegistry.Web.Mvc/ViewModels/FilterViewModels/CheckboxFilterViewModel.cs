using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

namespace DfE.EducationProviderRegistry.Web.Mvc.ViewModels;

public class CheckboxFilterViewModel : FilterViewModel
{
    public FacetViewModel Facet { get; init; }
}