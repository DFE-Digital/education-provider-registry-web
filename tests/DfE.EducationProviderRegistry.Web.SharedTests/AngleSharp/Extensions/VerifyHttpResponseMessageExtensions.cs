using AngleSharp.Html.Dom;
using System.Net;
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

        return await HtmlHelpers.GetDocumentAsync(httpResponse);
    }
}
