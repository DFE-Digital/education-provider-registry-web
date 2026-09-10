using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using DfE.EducationProviderRegistry.Web.Mvc.Settings;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Analytics.GoogleTagManger;

internal sealed class GoogleAnalyticsComponent
{
    private readonly IHtmlDocument _doc;
    private readonly GoogleAnalyticsSettings _settings;

    public GoogleAnalyticsComponent(IHtmlDocument doc, GoogleAnalyticsSettings settings)
    {
        ArgumentNullException.ThrowIfNull(doc);
        ArgumentNullException.ThrowIfNull(settings);
        _doc = doc;
        _settings = settings;
    }

    public bool Exists()
    {
        return TagManagerJavascriptTag is not null &&
            NoJavascriptTag is not null;
    }

    private IElement? TagManagerJavascriptTag =>
        _doc.QuerySelectorAll("script")
            .SingleOrDefault((scriptTag) =>
                scriptTag.InnerHtml.Contains(_settings.Domain));

    private IElement? NoJavascriptTag => _doc.QuerySelector($"noscript iframe[title='Google Tag Manager']");
}