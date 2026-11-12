namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.Filters;
public sealed record SelectedFilterViewModel(
    string FilterKey,
    string? Value,
    string Label,
    string AccessibleLabel,
    string RemoveValue);
