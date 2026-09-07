using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness.Anglesharp;
using Microsoft.AspNetCore.Http;
using HttpMethod = System.Net.Http.HttpMethod;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness.Anglesharp.Extensions;

internal static class HtmlFormExtensions
{
    public static HttpRequestMessage ToHttpRequestMessage(this HtmlForm form)
    {
        ArgumentNullException.ThrowIfNull(form);

        if (form.Method == HttpMethod.Get)
        {
            QueryString query =
                QueryString.Create(
                    form.Fields.Select(x =>
                        new KeyValuePair<string, string?>(
                            x.Key,
                            x.Value)));

            return new HttpRequestMessage(
                HttpMethod.Get,
                $"{form.Action}{query}");
        }

        return new HttpRequestMessage(
            form.Method,
            form.Action)
        {
            Content = new FormUrlEncodedContent(form.Fields)
        };
    }
}