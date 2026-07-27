using Microsoft.AspNetCore.Mvc.Rendering;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.FilterViewModels;

public sealed class AutocompleteFilterViewModel : FilterViewModel
{
    public string? SelectedValue { get; init; }

    public IReadOnlyCollection<SelectListItem> Options { get; init; } = [];
}