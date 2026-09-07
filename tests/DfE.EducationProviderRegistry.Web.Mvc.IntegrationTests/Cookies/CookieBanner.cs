using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Extensions;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Cookies;

internal sealed class CookieBanner
{
    private readonly IHtmlDocument _document;
    private const string CookieBannerSelector = ".govuk-cookie-banner";

    public CookieBanner(IHtmlDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        _document = document;
    }

    public bool Exists() => GetCookieBannerContainer is not null;

    private IElement? GetCookieBannerContainer => _document.QuerySelector(CookieBannerSelector);

    public HttpRequestMessage SubmitCookieBannerHttpRequest(
        bool addAntiForgeryToken = false,
        bool analytics = false)
    {
        if (GetCookieBannerContainer is null)
        {
            throw new InvalidOperationException($"Could not find CookieBanner with {CookieBannerSelector}");
        }

        IElement formContainer = GetCookieBannerContainer!.Closest("form") ??
            throw new ArgumentException("Could not find form for CookieBanner");

        IHtmlCollection<IElement> formElementsWithValue =
            _document.QuerySelectorAll(".govuk-cookie-banner [value]") ??
                throw new ArgumentException("Could not find any elements in CookieBanner with a value attribute");

        // Ensure element in form has value
        string targetValue = analytics ? "true" : "false";

        IElement element =
            formElementsWithValue.Single((t) =>
                t.GetAttribute("value") == targetValue);

        string targetParam = element.GetAttribute("name") ??
            throw new ArgumentException("Could not find name attribute on element");


        Dictionary<string, string?> formData = new()
        {
            [targetParam] = element.GetAttribute("value"),
        };

        // TODO application parts and remove

        if (addAntiForgeryToken)
        {
            IElement antiForgeryElement =
                formContainer.QuerySelector("input[name='__RequestVerificationToken']") ??
                    throw new ArgumentException("Could not find antiforgery token input");

            string antiForgeryToken =
                antiForgeryElement.GetAttribute("value") ??
                    throw new ArgumentException("Could not find antiforgery token value");

            formData["__RequestVerificationToken"] = antiForgeryToken;
        }

        (HttpMethod method, string? action) = formContainer.ParseFormAttributes();

        HttpRequestMessage request = new(method, action)
        {
            Content = new FormUrlEncodedContent(formData)
        };

        return request;
    }
}