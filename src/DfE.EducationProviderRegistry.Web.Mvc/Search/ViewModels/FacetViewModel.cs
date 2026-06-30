namespace DfE.EducationProviderRegistry.Web.Mvc.Search.ViewModels;

public record FacetViewModel(
    string Name, // e.g., "Establishment status"
    List<FacetValueViewModel> Values
);