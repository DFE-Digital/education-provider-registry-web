using DfE.Document.Abstractions;

namespace DfE.EducationProviderRegistry.Web.PageComponents.Pages;

public sealed class CookieBannerComponent
{
    private const string CookieBannerSelector = ".govuk-cookie-banner";
    private const string CookieBannerFormSelector = $"form:has({CookieBannerSelector})";

    private readonly ISearchContext _context;

    public CookieBannerComponent(ISearchContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;
    }

    public bool Exists() => _context.Find(CookieBannerSelector) is not null;

    public HtmlForm GetForm(bool analytics)
    {
        IElement form = GetForm();

        Dictionary<string, string> fields = [];

        IElement analyticsElement =
            form.FindAll("[name='analytics']")
                .Single(element =>
                    element.Attribute("value") == (analytics ? "true" : "false"));

        string analyticsName =
            analyticsElement.Attribute("name") ??
                throw new InvalidOperationException("Could not find analytics field name.");

        string analyticsValue =
            analyticsElement.Attribute("value") ??
                throw new InvalidOperationException("Could not find analytics field value.");

        fields.Add(analyticsName, analyticsValue);

        return new HtmlForm(
            Action: new Uri(
                form.Attribute("action") ??
                    throw new InvalidOperationException("Could not find form action."),
                UriKind.RelativeOrAbsolute),
            Method: new HttpMethod(
                form.Attribute("method")?.ToUpperInvariant() ?? "GET"),
            Enctype: form.Attribute("enctype"),
            Fields: fields);
    }

    private IElement GetForm()
    {
        return _context.Find(CookieBannerFormSelector) ??
            throw new InvalidOperationException($"Could not find form for {CookieBannerSelector}");
    }
}
