using System.Net.Http.Json;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness.Antiforgery.Extensions;

internal static class HttpClientExtensions
{
    internal static async Task<AntiForgeryContext> GetAntiforgeryTokensAsync(
        this HttpClient client,
        CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response =
            await client.GetAsync(
                "/_test/antiforgery",
                cancellationToken);

        response.EnsureSuccessStatusCode();

        AntiForgeryRequestToken dto =
            await response.Content.ReadFromJsonAsync<AntiForgeryRequestToken>(
                cancellationToken)
            ?? throw new InvalidOperationException();

        const string aspAntiForgeryCookieName = ".AspNetCore.Antiforgery";
        string antiForgeryCookieValue;

        // If the request already has a cookie (which it could obtain from above call)

        // the response won't contain another AntiForgeryCookie through Set-Cookie and rather the client will reuse the existing
        // (e.g. over HTTP Secure Cookies are not transmitted)

        // Parse either to get a valid SessionCookie

        if (response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string>? cookies))
        {
            antiForgeryCookieValue = cookies
                .Single(x => x.Contains(aspAntiForgeryCookieName))
                .Split(';')[0];
        }
        else
        {
            antiForgeryCookieValue = response.RequestMessage?.Headers
                .GetValues("Cookie")
                .Single()
                .Split(';')
                .Single(x => x.Contains(aspAntiForgeryCookieName))
                .Trim() ??
                    throw new InvalidOperationException("Could not find antiforgery cookie.");
        }

        return new AntiForgeryContext(
            dto.FormFieldName,
            dto.RequestToken,
            antiForgeryCookieValue);
    }
}