namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.Filters;

public interface IFilterViewModel
{
    string Name { get; }
    string Label { get; }
    IReadOnlyList<SelectedFilterValueViewModel> SelectedValues { get; }
}

public sealed record SelectedFilterValueViewModel(string? Value, string Label);
