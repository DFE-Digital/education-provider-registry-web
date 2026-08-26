using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Io;
using System.Net.Http.Headers;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests;

internal static class HtmlHelpers
{
    public static async Task<IHtmlDocument> GetDocumentAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        var document = await BrowsingContext.New()
            .OpenAsync(ResponseFactory, CancellationToken.None);
        return (IHtmlDocument)document;

        void ResponseFactory(VirtualResponse htmlResponse)
        {
            if (response.RequestMessage is not null)
            {
                htmlResponse.Address(response.RequestMessage.RequestUri);
            }

            htmlResponse.Status(response.StatusCode);

            MapHeaders(response.Headers);
            MapHeaders(response.Content.Headers);

            htmlResponse.Content(content);

            void MapHeaders(HttpHeaders headers)
            {
                foreach (var header in headers)
                {
                    foreach (var value in header.Value)
                    {
                        htmlResponse.Header(header.Key, value);
                    }
                }
            }
        }
    }
}