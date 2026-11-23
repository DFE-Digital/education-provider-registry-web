using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.Request;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.Results;

public sealed record NumberRangeFilterViewModel(
    string Name,
    string Label,
    NumberRangeInputFilterViewModel Range) : IFilterViewModel
{
    public IReadOnlyList<SelectedFilterValueViewModel> SelectedValues =>
        Range.SelectedLabel is null ?
            [] :
                [new SelectedFilterValueViewModel(Value: null, Label: Range.SelectedLabel)];
}
