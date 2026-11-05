using AngleSharp.Dom;
using DfE.EducationProviderRegistry.Web.SharedTests.Infrastructure.Document.Abstractions;

namespace DfE.Document.Anglesharp;

public static class SearchContext
{
    public static ISearchContext From(IParentNode document)
    {
        ArgumentNullException.ThrowIfNull(document);
        return new AngleSharpSearchContext(document);
    }
}
