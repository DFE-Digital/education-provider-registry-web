using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Resolvers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Store;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies.Factories;
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
                strategyFactory: BreadcrumbStrategyFactoryTestDouble.Mock().Object);

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
                strategyFactory: BreadcrumbStrategyFactoryTestDouble.Mock().Object);

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
                strategyFactory: BreadcrumbStrategyFactoryTestDouble.Mock().Object);

        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_Throws_When_StrategyFactoryIsNull()
    {
        Func<BreadcrumbResultFilter> construct = () =>
            new(
                routeResolver: BreadcrumbRouteResolverTestDouble.Mock().Object,
                titleResolver: BreadcrumbTitleResolverTestDouble.Mock().Object,
                journeyStore: BreadcrumbJourneyStoreTestDouble.Mock().Object,
                strategyFactory: null!);

        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public async Task OnResultExecutionAsync_When_ResultIsNotViewResult_CallsNextWithoutTracking()
    {
        // arrange
        Mock<IBreadcrumbJourneyStore> journeyStore =
            BreadcrumbJourneyStoreTestDouble.Mock();

        Mock<IBreadcrumbStrategyFactory> strategyFactory = BreadcrumbStrategyFactoryTestDouble.Mock();

        BreadcrumbResultFilter sut =
            new(
                routeResolver: BreadcrumbRouteResolverTestDouble.Mock().Object,
                titleResolver: BreadcrumbTitleResolverTestDouble.Mock().Object,
                journeyStore: journeyStore.Object,
                strategyFactory: strategyFactory.Object);

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

        strategyFactory.Verify(
            factory => factory.GetStrategy(
                It.IsAny<BreadcrumbRouteRule>()),
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

        Mock<IBreadcrumbStrategyFactory> strategyFactory =
            BreadcrumbStrategyFactoryTestDouble.Mock();

        BreadcrumbResultFilter sut =
            new(
                routeResolver.Object,
                BreadcrumbTitleResolverTestDouble.Mock().Object,
                journeyStore.Object,
                strategyFactory.Object);

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

        journeyStore.Verify(
            store => store.Load(),
            Times.Never);

        strategyFactory.Verify(
            factory => factory.GetStrategy(
                It.IsAny<BreadcrumbRouteRule>()),
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

        Mock<IBreadcrumbStrategyFactory> strategyFactory =
            BreadcrumbStrategyFactoryTestDouble.MockFor(
                BreadcrumbStrategyTestDouble.Mock().Object);

        BreadcrumbResultFilter sut =
            new(
                routeResolver.Object,
                titleResolver.Object,
                journeyStore.Object,
                strategyFactory.Object);

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

        Mock<IBreadcrumbStrategyFactory> strategyFactory =
            BreadcrumbStrategyFactoryTestDouble.MockFor(
                BreadcrumbStrategyTestDouble.Mock().Object);

        BreadcrumbResultFilter sut =
            new(
                BreadcrumbRouteResolverTestDouble.MockFor(rule).Object,
                BreadcrumbTitleResolverTestDouble.MockFor("Title").Object,
                journeyStore.Object,
                strategyFactory.Object);

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
        // Arrange
        BreadcrumbRouteRule rule = new()
        {
            JourneyResetMode = BreadcrumbJourneyResetMode.DirectNavigationOnly
        };

        Mock<IBreadcrumbJourneyStore> journeyStore =
            BreadcrumbJourneyStoreTestDouble.MockFor(new BreadcrumbJourney());

        Mock<IBreadcrumbStrategyFactory> strategyFactory =
            BreadcrumbStrategyFactoryTestDouble.MockFor(
                BreadcrumbStrategyTestDouble.Mock().Object);

        BreadcrumbResultFilter sut =
            new(
                BreadcrumbRouteResolverTestDouble.MockFor(rule).Object,
                BreadcrumbTitleResolverTestDouble.MockFor("Title").Object,
                journeyStore.Object,
                strategyFactory.Object);

        ResultExecutingContext context =
            CreateContext(new ViewResult());

        context.HttpContext.Request.Host =
            new HostString("education-provider.service.gov.uk");

        context.HttpContext.Request.Headers.Referer =
            "https://education-provider.service.gov.uk/search";

        // Act
        await sut.OnResultExecutionAsync(
            context,
            CreateNextDelegate());

        // Assert
        journeyStore.Verify(
            store => store.Clear(),
            Times.Never);
    }

    [Fact]
    public async Task OnResultExecutionAsync_LoadsJourney_AppliesStrategy_SavesJourney()
    {
        // Arrange
        BreadcrumbJourney journey = new();

        BreadcrumbRouteRule rule = new();

        Mock<IBreadcrumbJourneyStore> journeyStore =
            BreadcrumbJourneyStoreTestDouble.MockFor(journey);

        Mock<IBreadcrumbStrategy> strategy =
            BreadcrumbStrategyTestDouble.Mock();

        Mock<IBreadcrumbStrategyFactory> strategyFactory =
            BreadcrumbStrategyFactoryTestDouble.MockFor(strategy.Object);

        Mock<IBreadcrumbTitleResolver> titleResolver =
            BreadcrumbTitleResolverTestDouble.MockFor("Test Title");

        BreadcrumbResultFilter sut =
            new(
                BreadcrumbRouteResolverTestDouble.MockFor(rule).Object,
                titleResolver.Object,
                journeyStore.Object,
                strategyFactory.Object);

        ResultExecutingContext context =
            CreateContext(new ViewResult());

        context.HttpContext.Request.Path = "/search";
        context.HttpContext.Request.QueryString = new QueryString("?page=1");

        bool nextCalled = false;

        // Act
        await sut.OnResultExecutionAsync(
            context,
            () =>
            {
                nextCalled = true;
                return CreateNextDelegate()();
            });

        // Assert
        Assert.True(nextCalled);

        titleResolver.Verify(
            resolver => resolver.Resolve(
                rule,
                It.IsAny<ViewResult>()),
            Times.Once);

        strategyFactory.Verify(
            factory => factory.GetStrategy(rule),
            Times.Once);

        strategy.Verify(
            strategy => strategy.Apply(
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
