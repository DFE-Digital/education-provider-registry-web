using DfE.Document.Abstractions;

namespace DfE.Document.Anglesharp;

internal sealed class AngleSharpElement : IElement
{
    private readonly AngleSharp.Dom.IElement _element;

    public AngleSharpElement(AngleSharp.Dom.IElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        _element = element;
    }

    public string Text => _element.TextContent;

    public string? Attribute(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return _element.GetAttribute(name);
    }

    public IElement? Find(string selector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);

        AngleSharp.Dom.IElement? descendant =
            _element.QuerySelector(selector);

        return descendant is null
            ? null
            : new AngleSharpElement(descendant);
    }

    public IReadOnlyCollection<IElement> FindAll(string selector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);

        return _element
            .QuerySelectorAll(selector)
            .Select(descendant => (IElement)new AngleSharpElement(descendant))
            .ToArray();
    }
}
