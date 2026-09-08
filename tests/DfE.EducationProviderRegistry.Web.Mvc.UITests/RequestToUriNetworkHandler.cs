using OpenQA.Selenium;

namespace DfE.EducationProviderRegistry.Web.MVC.UITests;

internal sealed class RequestToUriNetworkHandler : NetworkRequestHandler
{
    internal static HttpRequestData RouteUrlToUnknownDomain(HttpRequestData data)
    {
        // Sink off to invalid domain - browser should ignore or failed DNS
        // RFC 2606 reserves .invalid TLD
        // RFC 6761 documents how special-use domains should be treated
        data.Url = "somewhere.invalid";
        return data;
    }

    public RequestToUriNetworkHandler(string url, Func<HttpRequestData, HttpRequestData>? transformer = null)
    {
        RequestMatcher = (httpData) =>
        {
            bool match = httpData.Url?.Contains(url, StringComparison.OrdinalIgnoreCase) ?? false;
            if (match)
            {
                RequestsMatchCounter++;
            }
            return match;
        };

        RequestTransformer = transformer;

        // Empty

        RequestsMatchCounter = 0;
    }

    public int RequestsMatchCounter { get; private set; }
}