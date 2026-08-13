namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

public record FacetValueViewModel(
    string Value,
    string Label,
    long? Count,
    bool IsSelected
);
