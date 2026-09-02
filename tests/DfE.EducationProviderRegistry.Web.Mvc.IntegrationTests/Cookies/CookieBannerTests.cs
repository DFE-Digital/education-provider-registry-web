using AngleSharp.Html.Dom;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Extensions;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Search;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using System.Text;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Cookies;

public sealed class CookieBannerTests : WebApplicationFactoryBaseIntegrationTest
{
    public CookieBannerTests(IServiceProvider provider) : base(provider)
    {
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/cookies")]
    [InlineData("/search")]
    public async Task View_Any_Route_Displays_Cookie_Banner(string path)
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;
        using HttpClient client = Factory.CreateDefaultedHttpClient();

        // Act
        using HttpResponseMessage response = await client.GetAsync(path, ct);

        // Assert
        IHtmlDocument doc = await response.AssertSuccessfulHtmlResponseAsync();

        CookieBanner banner = new(doc);
        Assert.True(banner.Exists());
    }
}

internal sealed class CookieBanner
{
    private readonly IHtmlDocument _document;

    public CookieBanner(IHtmlDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        _document = document;
    }

    public bool Exists()
    {
        return _document.QuerySelector(".govuk-cookie-banner") is not null;
    }
}