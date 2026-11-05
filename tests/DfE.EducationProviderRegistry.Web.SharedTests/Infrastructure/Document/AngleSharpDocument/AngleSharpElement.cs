using IElement = AngleSharp.Dom.IElement;

namespace DfE.EducationProviderRegistry.Web.SharedTests.Infrastructure.Document.AngleSharpDocument;

internal class AngleSharpElement : DfE.Document.Abstractions.IElement
{
    private readonly IElement _element;

    public AngleSharpElement(IElement element)
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

    public DfE.Document.Abstractions.IElement? Find(string selector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);

        IElement? element = _element.QuerySelector(selector);

        return element is null
            ? null
            : new AngleSharpElement(element);
    }

    public IReadOnlyCollection<DfE.Document.Abstractions.IElement> FindAll(string selector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);

        return _element
            .QuerySelectorAll(selector)
            .Select(element => (DfE.Document.Abstractions.IElement)new AngleSharpElement(element))
            .ToList();
    }
}
