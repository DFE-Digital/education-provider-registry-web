using DfE.Document.Abstractions;

namespace DfE.EducationProviderRegistry.Web.PageComponents.Commands;

public sealed record class ClickCommand : ICommand<ClickCommandModel>
{

}

public sealed record class ClickCommandModel
{
    public required string Selector { get; init; }
    public string? WithText { get; init; }
}


public enum Target
{
    Selector = 1,
    Text = 2
}

public enum SelectorType
{
    Css,
    XPath
}
