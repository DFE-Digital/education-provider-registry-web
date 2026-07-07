using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests;

public sealed class ResourceTests
{
    private readonly CancellationToken _ct;

    public ResourceTests()
    {
        _ct = TestContext.Current.CancellationToken;
    }

    [Theory]

    // *** CSS ***
    // GOVUK
    [InlineData("/css/govuk-frontend.min.css", "text/css")]
    // MOJ
    [InlineData("/css/moj-frontend.min.css", "text/css")]
    // *** Javascript ***
    // GOVUK
    [InlineData("/js/govuk-frontend.min.js", "text/javascript")]
    // MOJ
    [InlineData("/js/moj-frontend.min.js", "text/javascript")]
    // *** Fonts ***
    // GOVUK
    [InlineData("/assets/fonts/bold-b542beb274-v2.woff2", "font/woff2")]
    [InlineData("/assets/fonts/bold-affa96571d-v2.woff", "application/font-woff")]
    [InlineData("/assets/fonts/light-94a07e06a1-v2.woff2", "font/woff2")]
    [InlineData("/assets/fonts/light-f591b13f7d-v2.woff", "application/font-woff")]
    // *** Images ***
    // GOVUK
    [InlineData("/assets/images/favicon.ico", "image/x-icon")]
    [InlineData("/assets/images/favicon.svg", "image/svg+xml")]
    [InlineData("/assets/images/govuk-crest.svg", "image/svg+xml")]
    [InlineData("/assets/images/govuk-icon-180.png", "image/png")]
    [InlineData("/assets/images/govuk-icon-192.png", "image/png")]
    [InlineData("/assets/images/govuk-icon-mask.svg", "image/svg+xml")]
    [InlineData("/assets/images/govuk-opengraph-image.png", "image/png")]

    // *** JSON ***
    // GOVUK
    [InlineData("/assets/manifest.json", "application/json")]
    public async Task Get_Resource_ShouldReturnExpectedResult(string path, string expectedMime)
    {
        // Arrange
        using WebApplicationFactory<Program> factory = new();
        using HttpClient client = factory.CreateClient();

        // Act
        HttpResponseMessage response = await client.GetAsync(path, _ct);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(expectedMime, response.Content.Headers.ContentType!.MediaType!.ToString());
    }
}
