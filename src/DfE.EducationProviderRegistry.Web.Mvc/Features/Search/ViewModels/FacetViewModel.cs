namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

/// Example:
/// <code>
/// Name = "Phase"
/// Values = [
///     new FacetValueViewModel("Primary", 120, false),
///     new FacetValueViewModel("Secondary", 80, true)
/// ]
/// </code>
/// </remarks>
public record FacetViewModel(
    string Name,
    List<FacetValueViewModel> Values
);
