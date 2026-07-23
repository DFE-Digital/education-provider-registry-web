using Microsoft.AspNetCore.Mvc.Rendering;

namespace DfE.EducationProviderRegistry.Web.Mvc.ViewModels;

public sealed class AutocompleteFilterViewModel : FilterViewModel
{
    public string? SelectedValue { get; init; }

    public IReadOnlyCollection<SelectListItem> Options { get; init; } = [];
}