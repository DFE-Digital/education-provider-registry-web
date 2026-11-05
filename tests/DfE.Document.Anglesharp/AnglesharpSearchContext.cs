using AngleSharp.Dom;
using DfE.EducationProviderRegistry.Web.SharedTests.Infrastructure.Document.Abstractions;
using IElement = DfE.Document.Abstractions.IElement;

namespace DfE.Document.Anglesharp;

public sealed class AngleSharpSearchContext : ISearchContext
{
    private readonly IParentNode _searchContext;

    public AngleSharpSearchContext(IParentNode searchContext)
    {
        ArgumentNullException.ThrowIfNull(searchContext);

        _searchContext = searchContext;
    }

    public IElement? Find(string selector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);

        AngleSharp.Dom.IElement? element =
            _searchContext.QuerySelector(selector);

        return element is null
            ? null
            : new AngleSharpElement(element);
    }

    public IReadOnlyCollection<IElement> FindAll(string selector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);

        return _searchContext
            .QuerySelectorAll(selector)
            .Select(element => new AngleSharpElement(element))
            .ToArray();
    }
}
