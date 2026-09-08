using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness.Anglesharp;
using HttpMethod = System.Net.Http.HttpMethod;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Cookies;

internal sealed class CookieBanner
{
    private const string CookieBannerSelector = ".govuk-cookie-banner";
    private readonly IHtmlDocument _document;

    public CookieBanner(IHtmlDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        _document = document;
    }

    public bool Exists() => GetContainer() is not null;

    public HtmlForm GetForm(bool analytics)
    {
        IHtmlFormElement form = GetForm();

        Dictionary<string, string> fields = [];

        IElement analyticsElement =
            form.Elements
                .OfType<IElement>()
                .Single(t =>
                    t.GetAttribute("value") == (analytics ? "true" : "false"));

        string analyticsName =
            analyticsElement.GetAttribute("name") ??
                throw new InvalidOperationException("Could not find analytics field name.");

        string analyticsValue =
            analyticsElement.GetAttribute("value") ??
                throw new InvalidOperationException("Could not find analytics field value.");

        fields.Add(analyticsName, analyticsValue);

        return new HtmlForm(
            Action: new Uri(form.Action, UriKind.RelativeOrAbsolute),
            Method: new HttpMethod(form.Method.ToUpperInvariant()),
            Enctype: form.Enctype,
            Fields: fields);
    }

    private IElement? GetContainer() => _document.QuerySelector(CookieBannerSelector);

    private IHtmlFormElement GetForm()
    {
        return GetContainer()?.Closest("form") as IHtmlFormElement ??
            throw new InvalidOperationException($"Could not find form for {CookieBannerSelector}");
    }
}