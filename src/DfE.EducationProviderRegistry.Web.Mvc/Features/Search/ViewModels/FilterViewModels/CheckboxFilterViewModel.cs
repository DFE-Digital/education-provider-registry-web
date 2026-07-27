using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.FilterViewModels;

public class CheckboxFilterViewModel : FilterViewModel
{
    public FacetViewModel Facet { get; init; }
}