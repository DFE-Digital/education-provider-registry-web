using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies.Factories;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Tracking;
using DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Filters.Breadcrumbs.TestDoubles;
using Moq;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Filters.Breadcrumbs.Tracking;

public sealed class BreadcrumbTrackerTests
{
    [Fact]
    public void Constructor_Throws_When_BreadcrumbStrategyFactoryIsNull()
    {
        // arrange
        Func<BreadcrumbTracker> construct = () =>
            new(breadcrumbStrategyFactory: null!);

        // act & assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Track_Throws_When_JourneyIsNull()
    {
        // arrange
        BreadcrumbTracker sut =
            new(BreadcrumbStrategyFactoryTestDouble.Mock().Object);

        // act & assert
        Assert.Throws<ArgumentNullException>(
            () => sut.Track(
                null!,
                new BreadcrumbRouteRule(),
                new BreadcrumbItem("Details", "/details")));
    }

    [Fact]
    public void Track_Throws_When_RuleIsNull()
    {
        // arrange
        BreadcrumbTracker sut =
            new(BreadcrumbStrategyFactoryTestDouble.Mock().Object);

        // act & assert
        Assert.Throws<ArgumentNullException>(
            () => sut.Track(
                new BreadcrumbJourney(),
                null!,
                new BreadcrumbItem("Details", "/details")));
    }

    [Fact]
    public void Track_Throws_When_CurrentBreadcrumbItemIsNull()
    {
        // arrange
        BreadcrumbTracker sut =
            new(BreadcrumbStrategyFactoryTestDouble.Mock().Object);

        // act & assert
        Assert.Throws<ArgumentNullException>(
            () => sut.Track(
                new BreadcrumbJourney(),
                new BreadcrumbRouteRule(),
                null!));
    }

    [Fact]
    public void Track_GetsStrategyFromFactory_And_AppliesStrategy()
    {
        // arrange
        BreadcrumbJourney journey = new();
        BreadcrumbRouteRule rule = new();

        BreadcrumbItem currentBreadcrumbItem =
            new("Details", "/details");

        Mock<IBreadcrumbStrategy> strategy =
            BreadcrumbStrategyTestDouble.Mock();

        Mock<IBreadcrumbStrategyFactory> strategyFactory =
            BreadcrumbStrategyFactoryTestDouble.MockFor(
                strategy.Object);

        BreadcrumbTracker sut =
            new(strategyFactory.Object);

        // act
        sut.Track(
            journey,
            rule,
            currentBreadcrumbItem);

        // assert
        strategyFactory.Verify(
            factory =>
                factory.GetStrategy(rule),
            Times.Once);

        strategy.Verify(
            breadcrumbStrategy =>
                breadcrumbStrategy.Apply(
                    journey,
                    rule,
                    currentBreadcrumbItem),
            Times.Once);
    }
}
