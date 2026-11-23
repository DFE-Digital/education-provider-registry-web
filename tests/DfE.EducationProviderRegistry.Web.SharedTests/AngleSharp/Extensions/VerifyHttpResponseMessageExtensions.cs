using System.Net;
using System.Net.Http.Headers;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Io;
using Xunit;

namespace DfE.EducationProviderRegistry.Web.SharedTests.AngleSharp.Extensions;

public static class VerifyHttpResponseMessageExtensions
{
    public static Task<IHtmlDocument> AssertSuccessfulHtmlResponseAsync(this HttpResponseMessage httpResponse) =>
        AssertAndParseHttpResponseMessage(httpResponse, HttpStatusCode.OK);

    public static async Task<IHtmlDocument> AssertAndParseHttpResponseMessage(HttpResponseMessage httpResponse, HttpStatusCode statusCode)
    {
        Assert.True(
            httpResponse.IsSuccessStatusCode,
            $"Expected a status code: {statusCode} but got: {(int)httpResponse.StatusCode} {httpResponse.ReasonPhrase}");

        Assert.Equal("text/html; charset=utf-8", httpResponse.Content.Headers.ContentType!.ToString());

        return await GetDocumentAsync(httpResponse);
    }

    private static async Task<IHtmlDocument> GetDocumentAsync(HttpResponseMessage response)
    {
        string content = await response.Content.ReadAsStringAsync();

        IDocument document =
            await BrowsingContext.New().OpenAsync(ResponseFactory, CancellationToken.None);

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
                foreach (KeyValuePair<string, IEnumerable<string>> header in headers)
                {
                    foreach (string value in header.Value)
                    {
                        htmlResponse.Header(header.Key, value);
                    }
                }
            }
        }
    }
}
