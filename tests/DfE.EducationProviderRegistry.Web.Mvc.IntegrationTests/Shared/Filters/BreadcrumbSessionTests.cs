using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.PageComponents.AngleSharp;
using DfE.EducationProviderRegistry.Web.SharedTests.Infrastructure.Filters;
using DfE.EducationProviderRegistry.Web.SharedTests.TestDoubles;
using DfE.EducationProviderRegistry.Web.SharedTests.TestDoubles.Filters;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Shared.Filters;

public sealed class BreadcrumbSessionTests
{
    // This test fully integrates with the real Session for Breadcrumbs ensuring session retention and serialisation works across requests
    [Fact]
    public async Task Get_WhenCurrentPathMatchesExistingItem_WithDifferentQueryString_TruncatesJourney()
    {
        // Arrange
        BreadcrumbRouteRule homeRule =
            BreadcrumbRouteRuleBuilder.Create(pathPattern: "/")
                .WithTitle("Home")
                .Build();

        BreadcrumbRouteRule newRule =
            BreadcrumbRouteRuleBuilder.Create(pathPattern: "/search")
                .WithTitle("New")
                .Build();

        BreadcrumbOptions options =
            BreadcrumbOptionsTestDoubles.Create(
                [
                    homeRule,
                    newRule
                ]);

        using WebApplicationFactory<Program> factory =
            WebApplicationFactoryProvider.Default
                .WithWebHostBuilder((builder) =>
                    builder.UseBreadcrumbConfiguration(options));

        using HttpClient client = factory.CreateClient();

        using HttpRequestMessage homeRequest =
            HttpRequestHelpers.CreateInternalReferrerRequest(client, targetPath: "/?id=1");

        using HttpResponseMessage homeResponse =
            await client.SendAsync(homeRequest, TestContext.Current.CancellationToken);

        await homeResponse.AssertSuccessfulHtmlResponseAsync();

        using HttpRequestMessage newRequest =
            HttpRequestHelpers.CreateInternalReferrerRequest(client, targetPath: "/search");

        using HttpResponseMessage newResponse =
            await client.SendAsync(
                newRequest,
                TestContext.Current.CancellationToken);

        IHtmlDocument newDocument =
            await newResponse.AssertSuccessfulHtmlResponseAsync();

        BreadcrumbsComponent breadcrumb = new(newDocument);

        Breadcrumb homeBreadcrumb = Assert.Single(breadcrumb.Breadcrumbs());

        Assert.Equal("Home", homeBreadcrumb.Text);
        Assert.Equal("/?id=1", homeBreadcrumb.Link);

        // Act
        using HttpRequestMessage returnHomeRequest =
            HttpRequestHelpers.CreateInternalReferrerRequest(client, targetPath: "/?id=2");

        using HttpResponseMessage httpResponse =
            await client.SendAsync(returnHomeRequest, TestContext.Current.CancellationToken);

        // Assert
        IHtmlDocument document =
            await httpResponse.AssertSuccessfulHtmlResponseAsync();

        BreadcrumbsComponent component = new(document);

        Assert.Empty(component.Breadcrumbs());
    }
}
