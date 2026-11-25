using AngleSharp.Dom;
using DfE.Document.Abstractions;

namespace DfE.Document.Anglesharp;

public static class SearchContext
{
    public static ISearchContext From(IParentNode document)
    {
        ArgumentNullException.ThrowIfNull(document);
        return new AngleSharpSearchContext(document);
    }
}
