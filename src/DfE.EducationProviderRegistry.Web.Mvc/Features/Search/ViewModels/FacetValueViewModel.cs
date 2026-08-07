namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

public record FacetValueViewModel(
    string Value,
    string Id,
    long? Count,
    bool IsSelected
);
