namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

public record FacetValueViewModel(
    string Key,
    string Value,
    long? Count,
    bool IsSelected
);
