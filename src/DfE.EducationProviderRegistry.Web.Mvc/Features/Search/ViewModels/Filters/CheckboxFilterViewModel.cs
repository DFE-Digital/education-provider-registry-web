namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.Filters;

public sealed record CheckboxFilterViewModel(
    string Name,
    string Label,
    IReadOnlyList<FacetValueViewModel> Values) : IFilterViewModel
{
    public IReadOnlyList<SelectedFilterValueViewModel> SelectedValues =>
        [..Values
            .Where((value) => value.IsSelected)
            .Select((value) =>
                new SelectedFilterValueViewModel(
                    value.Value,
                    value.Label))];
}
