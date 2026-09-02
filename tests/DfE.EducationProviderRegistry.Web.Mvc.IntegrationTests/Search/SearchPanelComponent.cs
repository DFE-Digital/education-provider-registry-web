using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Search;

public sealed class SearchPanelComponent
{
    private readonly IHtmlDocument _document;

    public SearchPanelComponent(IHtmlDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        _document = document;
    }

    internal (HttpMethod method, Uri target) GetFormDetails()
    {
        IElement element = _document.QuerySelector("#main-content form")!;

        HttpMethod method = element.GetAttribute("method")?.ToLowerInvariant() switch
        {
            "post" => HttpMethod.Post,
            _ => HttpMethod.Get
        };

        Uri currentPageUri = new(_document.Url);

        string? actionAttribute = element.GetAttribute("action");

        Uri targetUri =
            string.IsNullOrWhiteSpace(actionAttribute)
                ? currentPageUri
                : new Uri(currentPageUri, actionAttribute);

        return
            (method, targetUri);
    }

    public string GetIdentityInputName()
    {
        return
            _document.QuerySelector("#SearchKeywords")?
                .GetAttribute("name") ??
                    throw new ArgumentException("#SearchKeywords attribute name does not exist");
        // get form
    }

    public string GetLocationInputName()
    {
        return
            _document.QuerySelector("#Address")?
                .GetAttribute("name") ??
                    throw new ArgumentException("#SearchKeywords attribute name does not exist");
    }
}