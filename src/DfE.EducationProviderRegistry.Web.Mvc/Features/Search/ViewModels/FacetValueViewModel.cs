namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

/// <summary>
/// Represents a single selectable value within a search facet, including its
/// display text, the number of matching records, and whether the value is
/// currently selected by the user.
/// </summary>
/// <param name="Value">
/// The facet value as displayed to the user.  
/// Example: <c>"Open, but proposed to close"</c>.
/// </param>
/// <param name="Count">
/// The number of records in the search results that match this facet value.  
/// Example: <c>150</c>.  
/// May be <c>null</c> if the count is unavailable.
/// </param>
/// <param name="IsSelected">
/// Indicates whether this facet value is currently selected by the user.  
/// Example: <c>true</c> when the filter is active.
/// </param>
public record FacetValueViewModel(
    string Value,
    long? Count,
    bool IsSelected
);
