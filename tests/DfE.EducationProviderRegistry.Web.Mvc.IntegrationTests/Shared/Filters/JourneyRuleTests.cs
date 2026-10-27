using AngleSharp.Dom;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Shared.Filters.Infrastructure.Extensions;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Shared.Filters.TestDoubles;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Shared.Filters;

public sealed class JourneyRuleTests
{
    [Fact]
    public async Task Get_WhenRuleHasBaseJourney_RendersBaseJourneyInConfiguredOrder()
    {
        // Arrange
        BreadcrumbRouteRule rule =
            BreadcrumbRouteRuleBuilder.Create(pathPattern: "/")
                .WithJourney(title: "Home1", url: "/home/1")
                .WithJourney(title: "home2", url: "/home2")
                .Build();

        BreadcrumbOptions options =
            BreadcrumbOptionsTestDoubles.Create([rule]);

        using WebApplicationFactory<Program> factory =
            WebApplicationFactoryProvider.Default
                .WithWebHostBuilder((builder) =>
                    builder.UseBreadcrumbConfiguration(options));

        using HttpClient client = factory.CreateClient();

        // Act
        using HttpResponseMessage httpResponse =
            await client.GetAsync(
                "/",
                TestContext.Current.CancellationToken);

        // Assert
        IHtmlDocument document =
            await httpResponse.AssertSuccessfulHtmlResponseAsync();

        IHtmlCollection<IElement> breadcrumbItems =
            document.QuerySelectorAll("#breadcrumbs-nav .govuk-breadcrumbs__list-item a");

        Assert.Collection(breadcrumbItems,
            (first) =>
            {
                Assert.Equal("Home1", first.TextContent.Trim());
                Assert.Equal("/home/1", first.GetAttribute("href"));
            },
            (second) =>
            {
                Assert.Equal("home2", second.TextContent.Trim());
                Assert.Equal("/home2", second.GetAttribute("href"));
            });
    }
}

public sealed class BreadcrumbsComponent
{
    private readonly IHtmlDocument _document;

    public BreadcrumbsComponent(IHtmlDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        _document = document;
    }
}

//public static class BreadcrumbJourneySessionTestDoubles
//{
//    public static
//}
