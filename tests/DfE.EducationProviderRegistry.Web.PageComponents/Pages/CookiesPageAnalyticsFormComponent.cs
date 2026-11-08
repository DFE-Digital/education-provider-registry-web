using DfE.Document.Abstractions;
using DfE.EducationProviderRegistry.Web.PageComponents;

namespace DfE.EducationProviderRegistry.Web.PageComponents.Pages;

public sealed class CookiesPageAnalyticsFormComponent
{
    private readonly ISearchContext _context;

    public CookiesPageAnalyticsFormComponent(ISearchContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;
    }

    private IElement AnalyticsYes =>
        _context.Find("#analytics-yes") ??
        throw new ArgumentException("Could not find analytics yes radio button.");

    private IElement AnalyticsNo =>
        _context.Find("#analytics-no") ??
        throw new ArgumentException("Could not find analytics no radio button.");

    private IElement Form =>
        _context.Find("#cookies-form") ??
        throw new ArgumentException("Could not find cookies form");

    public bool AcceptAnalyticsSelected()
    {
        return AnalyticsYes.Attribute("checked") is not null;
    }

    public bool RejectAnalyticsSelected()
    {
        return AnalyticsNo.Attribute("checked") is not null;
    }

    public HtmlForm GetForm(bool analytics)
    {
        IElement analyticsRadio = analytics ? AnalyticsYes : AnalyticsNo;

        string analyticsName =
            analyticsRadio.Attribute("name") ?? throw new InvalidOperationException("Could not find analytics field name.");

        string analyticsValue =
            analyticsRadio.Attribute("value") ?? throw new InvalidOperationException("Could not find analytics field value.");

        string action = Form.Attribute("action") ?? throw new InvalidOperationException("Could not find form action.");

        string method = Form.Attribute("method") ?? "GET";

        Dictionary<string, string> fields = new()
    {
        { analyticsName, analyticsValue }
    };

        return new HtmlForm(
            Action: new Uri(action, UriKind.RelativeOrAbsolute),
            Method: new HttpMethod(method.ToUpperInvariant()),
            Enctype: Form.Attribute("enctype"),
            Fields: fields);
    }
}
