using AngleSharp.Dom;

namespace DfE.EducationProviderRegistry.Web.SharedTests.Infrastructure.Document.AngleSharpDocument;

internal class AngleSharpElement : Abstractions.IElement
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

    public Abstractions.IElement? Find(string selector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);

        IElement? element = _element.QuerySelector(selector);

        return element is null
            ? null
            : new AngleSharpElement(element);
    }

    public IReadOnlyCollection<Abstractions.IElement> FindAll(string selector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);

        return _element
            .QuerySelectorAll(selector)
            .Select(element => (Abstractions.IElement)new AngleSharpElement(element))
            .ToList();
    }
}
