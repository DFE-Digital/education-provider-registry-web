namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

/// <summary>
/// Represents a single facet category in the search UI, including its display
/// name and the collection of available selectable values.
/// </summary>
/// <param name="Name">
/// The display name of the facet category shown to the user.  
/// Example: <c>"Establishment status"</c>.
/// </param>
/// <param name="Values">
/// The list of <see cref="FacetValueViewModel"/> items representing the
/// selectable values within this facet category.  
/// Each value includes its display text, result count, and selection state.
/// </param>
/// <remarks>
/// A <see cref="FacetViewModel"/> corresponds directly to a facet returned by the
/// search engine. The <paramref name="Name"/> identifies the facet, and
/// <paramref name="Values"/> contains the available filter options for that facet.
///
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
