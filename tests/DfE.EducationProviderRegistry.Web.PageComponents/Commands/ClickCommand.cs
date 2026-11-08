using DfE.Document.Abstractions;

namespace DfE.EducationProviderRegistry.Web.PageComponents.Commands;

public sealed record class ClickCommand : ICommand<ClickCommandModel>
{

}

public sealed record class ClickCommandModel
{
    public required string Selector { get; init; }
    // Temporary to facilitate clicking by text through XPath
    public SelectorType Type { get; init; } = SelectorType.Css;
}


public enum SelectorType
{
    Css,
    XPath
}
