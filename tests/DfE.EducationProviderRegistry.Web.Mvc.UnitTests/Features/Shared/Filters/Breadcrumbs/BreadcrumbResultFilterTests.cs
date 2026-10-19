using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Resolvers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Store;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Tracking;
using DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Filters.Breadcrumbs.TestDoubles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Filters.Breadcrumbs;

public sealed class BreadcrumbResultFilterTests
{
    [Fact]
    public void Constructor_Throws_When_RouteResolverIsNull()
    {
        Func<BreadcrumbResultFilter> construct = () =>
            new(
                routeResolver: null!,
                titleResolver: BreadcrumbTitleResolverTestDouble.Mock().Object,
                journeyStore: BreadcrumbJourneyStoreTestDouble.Mock().Object,
                tracker: BreadcrumbTrackerTestDouble.Mock().Object,
                navigationEvaluator: NavigationEvaluatorTestDouble.Mock().Object);

        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_Throws_When_TitleResolverIsNull()
    {
        Func<BreadcrumbResultFilter> construct = () =>
            new(
                routeResolver: BreadcrumbRouteResolverTestDouble.Mock().Object,
                titleResolver: null!,
                journeyStore: BreadcrumbJourneyStoreTestDouble.Mock().Object,
                tracker: BreadcrumbTrackerTestDouble.Mock().Object,
                navigationEvaluator: NavigationEvaluatorTestDouble.Mock().Object);

        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_Throws_When_JourneyStoreIsNull()
    {
        Func<BreadcrumbResultFilter> construct = () =>
            new(
                routeResolver: BreadcrumbRouteResolverTestDouble.Mock().Object,
                titleResolver: BreadcrumbTitleResolverTestDouble.Mock().Object,
                journeyStore: null!,
                tracker: BreadcrumbTrackerTestDouble.Mock().Object,
                navigationEvaluator: NavigationEvaluatorTestDouble.Mock().Object);

        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_Throws_When_TrackerIsNull()
    {
        Func<BreadcrumbResultFilter> construct = () =>
            new(
                routeResolver: BreadcrumbRouteResolverTestDouble.Mock().Object,
                titleResolver: BreadcrumbTitleResolverTestDouble.Mock().Object,
                journeyStore: BreadcrumbJourneyStoreTestDouble.Mock().Object,
                tracker: null!,
                navigationEvaluator: NavigationEvaluatorTestDouble.Mock().Object);

        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_Throws_When_NavigationEvaluatorIsNull()
    {
        Func<BreadcrumbResultFilter> construct = () =>
            new(
                routeResolver: BreadcrumbRouteResolverTestDouble.Mock().Object,
                titleResolver: BreadcrumbTitleResolverTestDouble.Mock().Object,
                journeyStore: BreadcrumbJourneyStoreTestDouble.Mock().Object,
                tracker: BreadcrumbTrackerTestDouble.Mock().Object,
                navigationEvaluator: null!);

        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public async Task OnResultExecutionAsync_When_ResultIsNotViewResult_CallsNextWithoutTracking()
    {
        // arrange
        Mock<IBreadcrumbJourneyStore> journeyStore =
            BreadcrumbJourneyStoreTestDouble.Mock();

        Mock<IBreadcrumbTracker> tracker =
            BreadcrumbTrackerTestDouble.Mock();

        BreadcrumbResultFilter sut =
            new(
                routeResolver: BreadcrumbRouteResolverTestDouble.Mock().Object,
                titleResolver: BreadcrumbTitleResolverTestDouble.Mock().Object,
                journeyStore: journeyStore.Object,
                tracker: tracker.Object,
                navigationEvaluator: NavigationEvaluatorTestDouble.Mock().Object);

        bool nextCalled = false;

        // act
        await sut.OnResultExecutionAsync(
            CreateContext(new EmptyResult()),
            () =>
            {
                nextCalled = true;
                return CreateNextDelegate()();
            });

        // assert
        Assert.True(nextCalled);

        journeyStore.Verify(store =>
            store.Load(),
            Times.Never);

        tracker.Verify(tracker =>
            tracker.Track(
                It.IsAny<BreadcrumbJourney>(),
                It.IsAny<BreadcrumbRouteRule>(),
                It.IsAny<BreadcrumbItem>()),
            Times.Never);
    }

    [Fact]
    public async Task OnResultExecutionAsync_When_RouteDoesNotResolve_CallsNextWithoutTracking()
    {
        // arrange
        Mock<IBreadcrumbRouteResolver> routeResolver =
            BreadcrumbRouteResolverTestDouble.MockNoMatch();

        Mock<IBreadcrumbJourneyStore> journeyStore =
            BreadcrumbJourneyStoreTestDouble.Mock();

        Mock<IBreadcrumbTracker> tracker =
            BreadcrumbTrackerTestDouble.Mock();

        BreadcrumbResultFilter sut =
            new(
                routeResolver.Object,
                BreadcrumbTitleResolverTestDouble.Mock().Object,
                journeyStore.Object,
                tracker.Object,
                NavigationEvaluatorTestDouble.Mock().Object);

        bool nextCalled = false;

        // act
        await sut.OnResultExecutionAsync(
            CreateContext(new ViewResult()),
            () =>
            {
                nextCalled = true;
                return CreateNextDelegate()();
            });

        // assert
        Assert.True(nextCalled);

        journeyStore.Verify(store =>
            store.Load(),
            Times.Never);

        tracker.Verify(tracker =>
            tracker.Track(
                It.IsAny<BreadcrumbJourney>(),
                It.IsAny<BreadcrumbRouteRule>(),
                It.IsAny<BreadcrumbItem>()),
            Times.Never);
    }

    [Fact]
    public async Task OnResultExecutionAsync_When_ResetModeAlways_ClearsJourney()
    {
        // arrange
        BreadcrumbRouteRule rule = new()
        {
            JourneyResetMode = BreadcrumbJourneyResetMode.Always
        };

        Mock<IBreadcrumbJourneyStore> journeyStore =
            BreadcrumbJourneyStoreTestDouble.MockFor(new BreadcrumbJourney());

        Mock<IBreadcrumbRouteResolver> routeResolver =
            BreadcrumbRouteResolverTestDouble.MockFor(rule);

        Mock<IBreadcrumbTitleResolver> titleResolver =
            BreadcrumbTitleResolverTestDouble.MockFor("Title");

        BreadcrumbResultFilter sut =
            new(
                routeResolver.Object,
                titleResolver.Object,
                journeyStore.Object,
                BreadcrumbTrackerTestDouble.Mock().Object,
                NavigationEvaluatorTestDouble.Mock().Object);

        // act
        await sut.OnResultExecutionAsync(
            CreateContext(new ViewResult()),
            CreateNextDelegate());

        // assert
        journeyStore.Verify(
            store => store.Clear(),
            Times.Once);
    }

    [Fact]
    public async Task OnResultExecutionAsync_When_DirectNavigationOnlyAndDirect_ClearsJourney()
    {
        // arrange
        BreadcrumbRouteRule rule = new()
        {
            JourneyResetMode = BreadcrumbJourneyResetMode.DirectNavigationOnly
        };

        Mock<IBreadcrumbJourneyStore> journeyStore =
            BreadcrumbJourneyStoreTestDouble.MockFor(new BreadcrumbJourney());

        BreadcrumbResultFilter sut =
            new(
                BreadcrumbRouteResolverTestDouble.MockFor(rule).Object,
                BreadcrumbTitleResolverTestDouble.MockFor("Title").Object,
                journeyStore.Object,
                BreadcrumbTrackerTestDouble.Mock().Object,
                NavigationEvaluatorTestDouble.MockFor(true).Object);

        // act
        await sut.OnResultExecutionAsync(
            CreateContext(new ViewResult()),
            CreateNextDelegate());

        // assert
        journeyStore.Verify(
            store => store.Clear(),
            Times.Once);
    }

    [Fact]
    public async Task OnResultExecutionAsync_When_DirectNavigationOnlyAndNotDirect_DoesNotClearJourney()
    {
        // arrange
        BreadcrumbRouteRule rule = new()
        {
            JourneyResetMode = BreadcrumbJourneyResetMode.DirectNavigationOnly
        };

        Mock<IBreadcrumbJourneyStore> journeyStore =
            BreadcrumbJourneyStoreTestDouble.MockFor(new BreadcrumbJourney());

        BreadcrumbResultFilter sut =
            new(
                BreadcrumbRouteResolverTestDouble.MockFor(rule).Object,
                BreadcrumbTitleResolverTestDouble.MockFor("Title").Object,
                journeyStore.Object,
                BreadcrumbTrackerTestDouble.Mock().Object,
                NavigationEvaluatorTestDouble.MockFor(false).Object);

        // act
        await sut.OnResultExecutionAsync(
            CreateContext(new ViewResult()),
            CreateNextDelegate());

        // assert
        journeyStore.Verify(
            store => store.Clear(),
            Times.Never);
    }

    [Fact]
    public async Task OnResultExecutionAsync_LoadsJourney_TracksBreadcrumb_SavesJourney()
    {
        // arrange
        BreadcrumbJourney journey = new();

        BreadcrumbRouteRule rule = new();

        Mock<IBreadcrumbJourneyStore> journeyStore =
            BreadcrumbJourneyStoreTestDouble.MockFor(journey);

        Mock<IBreadcrumbTracker> tracker =
            BreadcrumbTrackerTestDouble.Mock();

        Mock<IBreadcrumbTitleResolver> titleResolver =
            BreadcrumbTitleResolverTestDouble.MockFor("Test Title");

        BreadcrumbResultFilter sut =
            new(
                BreadcrumbRouteResolverTestDouble.MockFor(rule).Object,
                titleResolver.Object,
                journeyStore.Object,
                tracker.Object,
                NavigationEvaluatorTestDouble.Mock().Object);

        ResultExecutingContext context =
            CreateContext(new ViewResult());

        context.HttpContext.Request.Path = "/search";
        context.HttpContext.Request.QueryString =
            new QueryString("?page=1");

        bool nextCalled = false;

        // act
        await sut.OnResultExecutionAsync(
            context,
            () =>
            {
                nextCalled = true;
                return CreateNextDelegate()();
            });

        // assert
        Assert.True(nextCalled);

        titleResolver.Verify(
            resolver =>
                resolver.Resolve(
                    rule,
                    It.IsAny<ViewResult>()),
            Times.Once);

        tracker.Verify(
            track =>
                track.Track(
                    journey,
                    rule,
                    It.Is<BreadcrumbItem>(item =>
                        item.Title == "Test Title" &&
                        item.Url == "/search?page=1")),
            Times.Once);

        journeyStore.Verify(
            store => store.Save(journey),
            Times.Once);
    }

    private static ResultExecutingContext CreateContext(
        IActionResult result)
    {
        ActionContext actionContext =
            new(
                new DefaultHttpContext(),
                new RouteData(),
                new ActionDescriptor());

        return new ResultExecutingContext(
            actionContext,
            [],
            result,
            controller: null!);
    }

    private static ResultExecutionDelegate CreateNextDelegate()
    {
        return () =>
        {
            ActionContext actionContext =
                new(
                    new DefaultHttpContext(),
                    new RouteData(),
                    new ActionDescriptor());

            return Task.FromResult(
                new ResultExecutedContext(
                    actionContext,
                    [],
                    new EmptyResult(),
                    controller: null!));
        };
    }
}
