namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Extensions
{
    internal static class HttpRequestMessageExtensions
    {
        internal static void AddAspNetCoreAntiForgeryCookie(this HttpRequestMessage request, HttpResponseMessage response)
        {
            string antiForgeryCookie =
                response.Headers
                    .GetValues("Set-Cookie")
                    .Single((responseCookieSet) => responseCookieSet.Contains(".AspNetCore.Antiforgery"));

            request.Headers.Add(
                name: "Cookie",
                value: antiForgeryCookie.Split(';')[0]);
        }
    }

}
