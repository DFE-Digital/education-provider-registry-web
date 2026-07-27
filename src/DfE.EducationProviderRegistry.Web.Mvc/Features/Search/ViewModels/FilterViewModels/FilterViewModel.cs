namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.FilterViewModels;

public abstract class FilterViewModel
{
    public required string Name { get; init; }

    public required string BindingName { get; init; }

    public required string Label { get; init; }

    public string? Hint { get; init; }
}