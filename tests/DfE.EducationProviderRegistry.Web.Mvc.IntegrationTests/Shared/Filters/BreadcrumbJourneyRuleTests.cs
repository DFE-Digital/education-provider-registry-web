using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Store;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.PageComponents.AngleSharp;
using DfE.EducationProviderRegistry.Web.SharedTests.Infrastructure.Filters;
using DfE.EducationProviderRegistry.Web.SharedTests.TestDoubles;
using DfE.EducationProviderRegistry.Web.SharedTests.TestDoubles.Filters;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Shared.Filters;

public sealed class BreadcrumbJourneyRuleTests
{
    [Fact]
    public async Task Get_WhenRuleHasBaseJourney_RendersBaseJourneyInConfiguredOrder()
    {
        // Arrange
        BreadcrumbRouteRule rule =
            BreadcrumbRouteRuleBuilder.Create(pathPattern: "/")
                .WithAddToBaseJourney(title: "Home1", url: "/home/1")
                .WithAddToBaseJourney(title: "home2", url: "/home2")
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
            await client.GetAsync("/", TestContext.Current.CancellationToken);

        // Assert
        IHtmlDocument document =
            await httpResponse.AssertSuccessfulHtmlResponseAsync();

        BreadcrumbsComponent component = new(document);

        Assert.Collection(component.Breadcrumbs(),
            (first) =>
            {
                Assert.Equal("Home1", first.Text);
                Assert.Equal("/home/1", first.Link);
            },
            (second) =>
            {
                Assert.Equal("home2", second.Text);
                Assert.Equal("/home2", second.Link);
            });
    }

    [Fact]
    public async Task Get_WhenRuleHasBaseJourney_ReplacesExistingJourney()
    {
        // Arrange
        BreadcrumbRouteRule rule =
            BreadcrumbRouteRuleBuilder.Create(pathPattern: "/")
                .WithTitle("Current page")
                .WithAddToBaseJourney(title: "Home1", url: "/home/1")
                .WithAddToBaseJourney(title: "home2", url: "/home2")
                .Build();

        BreadcrumbOptions options =
            BreadcrumbOptionsTestDoubles.Create([rule]);

        BreadcrumbJourney journey =
            BreadcrumbJourneyBuilder.Create()
                .WithItem("Home", "/existing1")
                .WithItem("Search", "/search/2")
                .WithItem("Group", "/groups3")
                .Build();

        IBreadcrumbJourneyStore store =
            BreadcrumbJourneyStoreTestDouble.Stub(journey);

        using WebApplicationFactory<Program> factory =
            WebApplicationFactoryProvider.Default
                .WithWebHostBuilder((builder) =>
                    builder
                        .UseBreadcrumbConfiguration(options)
                        .UseBreadcrumbExistingJourney(store));

        using HttpClient client = factory.CreateClient();

        // Act
        using HttpResponseMessage httpResponse =
            await client.GetAsync(
                "/",
                TestContext.Current.CancellationToken);

        // Assert
        IHtmlDocument document =
            await httpResponse.AssertSuccessfulHtmlResponseAsync();

        // Assert breadcrumbs displayed
        BreadcrumbsComponent component = new(document);
        Assert.Collection(component.Breadcrumbs(),
            (first) =>
            {
                Assert.Equal("Home1", first.Text);
                Assert.Equal("/home/1", first.Link);
            },
            (second) =>
            {
                Assert.Equal("home2", second.Text);
                Assert.Equal("/home2", second.Link);
            });

        // Assert observable store
        Assert.Collection(store.Load().Items,
            (first) =>
            {
                Assert.Equal("Home1", first.Title);
                Assert.Equal("/home/1", first.Url);
            },
            (second) =>
            {
                Assert.Equal("home2", second.Title);
                Assert.Equal("/home2", second.Url);
            },
            (third) =>
            {
                Assert.Equal("Current page", third.Title);
                Assert.Equal("/", third.Url);
            });
    }

    [Fact]
    public async Task Get_WhenResetModeIsAlways_ClearsExistingJourney()
    {
        // Arrange
        BreadcrumbRouteRule rule =
            BreadcrumbRouteRuleBuilder.Create(pathPattern: "/")
                .WithTitle("Home")
                .WithJourneyResetMode(BreadcrumbJourneyResetMode.Always)
                .Build();

        BreadcrumbOptions options =
            BreadcrumbOptionsTestDoubles.Create([rule]);

        BreadcrumbJourney journey =
            BreadcrumbJourneyBuilder.Create()
                .WithItem("Existing 1", "/existing/1")
                .WithItem("Existing 2", "/existing/2")
                .Build();

        IBreadcrumbJourneyStore store =
            BreadcrumbJourneyStoreTestDouble.Stub(journey);

        using WebApplicationFactory<Program> factory =
            WebApplicationFactoryProvider.Default
                .WithWebHostBuilder((builder) =>
                    builder
                        .UseBreadcrumbConfiguration(options)
                        .UseBreadcrumbExistingJourney(store));

        using HttpClient client = factory.CreateClient();

        // Act
        using HttpResponseMessage httpResponse =
            await client.GetAsync("/", TestContext.Current.CancellationToken);

        // Assert
        IHtmlDocument document =
            await httpResponse.AssertSuccessfulHtmlResponseAsync();

        // Assert breadcrumbs displayed
        BreadcrumbsComponent component = new(document);
        Assert.Empty(component.Breadcrumbs());

        // Assert observable store
        BreadcrumbItem storedBreadcrumb = Assert.Single(store.Load().Items);
        Assert.Equal("Home", storedBreadcrumb.Title);
        Assert.Equal("/", storedBreadcrumb.Url);
    }

    [Fact]
    public async Task Get_WhenResetModeIsDirectNavigationOnly_AndRequestIsDirect_ReplacesExistingJourney()
    {
        // Arrange
        BreadcrumbRouteRule rule =
            BreadcrumbRouteRuleBuilder.Create(pathPattern: "/")
                .WithTitle("Home")
                .WithAddToBaseJourney("title1", url: "/new/1")
                .WithJourneyResetMode(BreadcrumbJourneyResetMode.DirectNavigationOnly)
                .Build();

        BreadcrumbOptions options =
            BreadcrumbOptionsTestDoubles.Create([rule]);

        BreadcrumbJourney existingJourney =
            BreadcrumbJourneyBuilder.Create()
                .WithItem("Existing 1", "/existing/1")
                .WithItem("Existing 2", "/existing/2")
                .Build();

        IBreadcrumbJourneyStore store = BreadcrumbJourneyStoreTestDouble.Stub(existingJourney);

        using WebApplicationFactory<Program> factory =
            WebApplicationFactoryProvider.Default
                .WithWebHostBuilder((builder) =>
                    builder
                        .UseBreadcrumbConfiguration(options)
                        .UseBreadcrumbExistingJourney(store));

        using HttpClient client = factory.CreateClient();

        // Act
        using HttpResponseMessage httpResponse =
            await client.GetAsync("/", TestContext.Current.CancellationToken);

        // Assert
        IHtmlDocument document =
            await httpResponse.AssertSuccessfulHtmlResponseAsync();


        // Assert Breadcrumbs displayed
        BreadcrumbsComponent component = new(document);
        Breadcrumb breadcrumb = Assert.Single(component.Breadcrumbs());

        Assert.Equal("title1", breadcrumb.Text);
        Assert.Equal("/new/1", breadcrumb.Link);

        // Assert observable store
        Assert.Collection(store.Load().Items,
            (first) =>
            {
                Assert.Equal("title1", first.Title);
                Assert.Equal("/new/1", first.Url);
            },
            (second) =>
            {
                Assert.Equal("Home", second.Title);
                Assert.Equal("/", second.Url);
            });
    }

    [Fact]
    public async Task Get_WhenResetModeIsDirectNavigationOnly_AndRequestIsInternal_PreservesExistingJourney()
    {
        // Arrange
        const string matchPath = "/";

        BreadcrumbRouteRule rule =
            BreadcrumbRouteRuleBuilder.Create(pathPattern: "/")
                .WithTitle("Home")
                .WithJourneyResetMode(BreadcrumbJourneyResetMode.DirectNavigationOnly)
                .Build();

        BreadcrumbOptions options =
            BreadcrumbOptionsTestDoubles.Create([rule]);

        BreadcrumbJourney existingJourney =
            BreadcrumbJourneyBuilder.Create()
                .WithItem("Existing 1", "/existing/1")
                .WithItem("Existing 2", "/existing/2")
                .Build();

        IBreadcrumbJourneyStore store = BreadcrumbJourneyStoreTestDouble.Stub(existingJourney);

        using WebApplicationFactory<Program> factory =
            WebApplicationFactoryProvider.Default
                .WithWebHostBuilder((builder) =>
                    builder
                        .UseBreadcrumbConfiguration(options)
                        .UseBreadcrumbExistingJourney(store));

        using HttpClient client = factory.CreateClient();

        using HttpRequestMessage request = HttpRequestHelpers.CreateInternalReferrerRequest(client, targetPath: matchPath);

        // Act
        using HttpResponseMessage httpResponse =
            await client.SendAsync(
                request,
                TestContext.Current.CancellationToken);

        // Assert
        IHtmlDocument document =
            await httpResponse.AssertSuccessfulHtmlResponseAsync();

        // Assert Breadcrumbs displayed
        BreadcrumbsComponent component = new(document);
        Assert.Collection(
            component.Breadcrumbs(),
            (first) =>
            {
                Assert.Equal("Existing 1", first.Text);
                Assert.Equal("/existing/1", first.Link);
            },
            (second) =>
            {
                Assert.Equal("Existing 2", second.Text);
                Assert.Equal("/existing/2", second.Link);
            });

        // Assert observable store
        Assert.Collection(store.Load().Items,
            (first) =>
            {
                Assert.Equal("Existing 1", first.Title);
                Assert.Equal("/existing/1", first.Url);
            },
            (second) =>
            {
                Assert.Equal("Existing 2", second.Title);
                Assert.Equal("/existing/2", second.Url);
            },
            (third) =>
            {
                Assert.Equal("Home", third.Title);
                Assert.Equal(matchPath, third.Url);
            });
    }

    [Fact]
    public async Task Get_WhenCurrentPathMatchesExistingItem_WithDifferentQueryString_TruncatesJourney()
    {
        // Arrange
        BreadcrumbRouteRule rule =
            BreadcrumbRouteRuleBuilder.Create(pathPattern: "/")
                .WithTitle("Current Home")
                .Build();

        BreadcrumbOptions options =
            BreadcrumbOptionsTestDoubles.Create([rule]);

        BreadcrumbJourney existingJourney =
            BreadcrumbJourneyBuilder.Create()
                .WithItem("Existing Home", "/?id=1")
                .WithItem("Search", "/search")
                .WithItem("Results", "/search/results")
                .Build();

        IBreadcrumbJourneyStore store = BreadcrumbJourneyStoreTestDouble.Stub(existingJourney);

        using WebApplicationFactory<Program> factory =
            WebApplicationFactoryProvider.Default
                .WithWebHostBuilder((builder) =>
                    builder
                        .UseBreadcrumbConfiguration(options)
                        .UseBreadcrumbExistingJourney(store));

        using HttpClient client = factory.CreateClient();

        using HttpRequestMessage request = HttpRequestHelpers.CreateInternalReferrerRequest(client, targetPath: "/?id=2");

        // Act
        using HttpResponseMessage httpResponse =
            await client.SendAsync(
                request,
                TestContext.Current.CancellationToken);

        // Assert
        IHtmlDocument document =
            await httpResponse.AssertSuccessfulHtmlResponseAsync();

        // Assert Breadcrumbs displayed
        BreadcrumbsComponent component = new(document);
        Assert.Empty(component.Breadcrumbs());

        // Assert observable store
        BreadcrumbItem storedBreadcrumb = Assert.Single(store.Load().Items);
        Assert.Equal("Current Home", storedBreadcrumb.Title);
        Assert.Equal("/?id=2", storedBreadcrumb.Url);
    }
}
