namespace DfE.EducationProviderRegistry.Web.SharedTests.TestDoubles;

public static class HttpRequestHelpers
{
    public static HttpRequestMessage CreateInternalReferrerRequest(HttpClient client, string targetPath)
    {
        HttpRequestMessage request = new(HttpMethod.Get, targetPath);
        request.Headers.Referrer = new Uri($"{client.BaseAddress}/any-url-referrer-from-internal");

        return request;
    }
}
