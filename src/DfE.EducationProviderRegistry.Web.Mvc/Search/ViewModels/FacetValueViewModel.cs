namespace DfE.EducationProviderRegistry.Web.Mvc.Search.ViewModels;

public record FacetValueViewModel(
    string Value,       // e.g., "Open, but proposed to close"
    long? Count,        // e.g., 150
    bool IsSelected     // e.g., true
);