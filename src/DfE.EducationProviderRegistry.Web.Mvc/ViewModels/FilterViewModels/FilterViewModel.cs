namespace DfE.EducationProviderRegistry.Web.Mvc.ViewModels;

public abstract class FilterViewModel
{
    public required string Name { get; init; }

    public required string BindingName { get; init; }

    public required string Label { get; init; }

    public string? Hint { get; init; }
}