using AngleSharp.Dom;
using DfE.EducationProviderRegistry.Web.SharedTests.Infrastructure.Document.Abstractions;

namespace DfE.EducationProviderRegistry.Web.SharedTests.Infrastructure.Document.AngleSharp;

internal class AngleSharpElementNode : Abstractions.IElement
{
    private readonly global::AngleSharp.Dom.IElement _element;

    public AngleSharpElementNode(global::AngleSharp.Dom.IElement element)
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

    public Abstractions.IElement? Find(string selector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);

        global::AngleSharp.Dom.IElement? element = _element.QuerySelector(selector);

        return element is null
            ? null
            : new AngleSharpElementNode(element);
    }

    public IReadOnlyCollection<Abstractions.IElement> FindAll(string selector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);

        return _element
            .QuerySelectorAll(selector)
            .Select(element => (IElement)new AngleSharpElementNode(element))
            .ToList();
    }
}
