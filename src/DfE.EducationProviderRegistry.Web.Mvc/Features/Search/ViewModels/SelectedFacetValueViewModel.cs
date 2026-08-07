namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels
{
    /// <summary>
    ///     Hold the display value and the relevant filter term for the under the hood searching
    /// </summary>
    /// <param name="FilterValue"></param>
    /// <param name="Value"></param>
    public record SelectedFacetValueViewModel(
        string? FilterValue,
        string Value);
}
