using AngleSharp.Dom;
using AngleSharp.Html.Parser;

namespace DfE.Document.Anglesharp.UnitTests;

internal static class AnglesSharpElementTestDoubles
{
    public static AngleSharpElement CreateElement(string html, string selector)
    {
        HtmlParser parser = new();
        IDocument document = parser.ParseDocument(html);

        AngleSharp.Dom.IElement? element =
            document.QuerySelector(selector);

        Assert.NotNull(element);

        return new AngleSharpElement(element);
    }
}
