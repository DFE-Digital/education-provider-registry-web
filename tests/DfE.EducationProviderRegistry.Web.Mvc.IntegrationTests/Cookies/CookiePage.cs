using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness.Anglesharp;
using HttpMethod = System.Net.Http.HttpMethod;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Cookies;

internal sealed class CookiesPageAnalyticsForm
{
    private readonly IHtmlDocument _document;

    public CookiesPageAnalyticsForm(IHtmlDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        _document = document;
    }

    private IHtmlInputElement AnalyticsYes =>
        _document.QuerySelector<IHtmlInputElement>("#analytics-yes") ??
            throw new ArgumentException("Could not find analytics yes radio button.");

    private IHtmlInputElement AnalyticsNo =>
        _document.QuerySelector<IHtmlInputElement>("#analytics-no") ??
            throw new ArgumentException("Could not find analytics no radio button.");

    private IHtmlFormElement Form =>
        _document.QuerySelector<IHtmlFormElement>("#cookies-form") ??
            throw new ArgumentException("Could not find cookies form");

    public bool AcceptAnalyticsSelected()
    {
        IHtmlInputElement? radio = Form.QuerySelector<IHtmlInputElement>("form #analytics-yes");
        return radio?.IsChecked ?? false;
    }

    public bool RejectAnalyticsSelected()
    {
        IHtmlInputElement? radio = Form.QuerySelector<IHtmlInputElement>("form #analytics-no");
        return radio?.IsChecked ?? false;
    }

    public HtmlForm GetForm(bool analytics)
    {
        IHtmlInputElement analyticsRadio =
            analytics ? AnalyticsYes : AnalyticsNo;

        Dictionary<string, string> fields = new()
        {
            { analyticsRadio.Name!, analyticsRadio.Value }
        };

        return new HtmlForm(
            Action: new Uri(Form.Action, UriKind.RelativeOrAbsolute),
            Method: new HttpMethod(Form.Method),
            Enctype: Form.Enctype,
            Fields: fields);
    }
}