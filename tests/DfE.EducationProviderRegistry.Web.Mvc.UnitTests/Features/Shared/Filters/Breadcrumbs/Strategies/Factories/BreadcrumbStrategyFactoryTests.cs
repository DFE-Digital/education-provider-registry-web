using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies.Factories;
using DfE.EducationProviderRegistry.Web.SharedTests.TestDoubles;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Filters.Breadcrumbs.Strategies.Factories;

public sealed class BreadcrumbStrategyFactoryTests
{
    [Fact]
    public void Constructor_WhenServiceProviderIsNull_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new BreadcrumbStrategyFactory(null!));
    }

    [Fact]
    public void GetStrategy_WhenBaseJourneyExists_ShouldReturnBaseJourneyStrategy()
    {
        // Arrange
        BaseJourneyStrategy expectedStrategy = new();

        ServiceProviderTestDouble serviceProvider =
        ServiceProviderTestDouble.Create();

        serviceProvider.AddKeyedService<IBreadcrumbStrategy>(
            "BaseJourney",
            expectedStrategy);

        BreadcrumbStrategyFactory sut = new(serviceProvider);

        BreadcrumbRouteRule rule = new()
        {
            BaseJourney =
            [
                new BreadcrumbPrefixItem
                {
                    Title = "Home",
                    Url = "/"
                }
            ]
        };

        // Act
        IBreadcrumbStrategy actual =
        sut.GetStrategy(rule);

        // Assert
        Assert.Same(expectedStrategy, actual);
    }

    [Fact]
    public void GetStrategy_WhenBaseJourneyIsEmpty_ShouldReturnIncrementalJourneyStrategy()
    {
        // Arrange
        IncrementalJourneyStrategy expectedStrategy = new([]);

        ServiceProviderTestDouble serviceProvider =
            ServiceProviderTestDouble.Create();

        serviceProvider.AddKeyedService<IBreadcrumbStrategy>(
            "IncrementalJourney",
            expectedStrategy);

        BreadcrumbStrategyFactory sut =
            new(serviceProvider);

        // Act
        IBreadcrumbStrategy actual =
            sut.GetStrategy(new BreadcrumbRouteRule());

        // Assert
        Assert.Same(expectedStrategy, actual);
    }

    [Fact]
    public void GetStrategy_WhenBaseJourneyStrategyIsNotRegistered_ShouldThrowInvalidOperationException()
    {
        // Arrange
        ServiceProviderTestDouble serviceProvider =
            ServiceProviderTestDouble.Create();

        BreadcrumbStrategyFactory sut =
            new(serviceProvider);

        BreadcrumbRouteRule rule = new()
        {
            BaseJourney =
            [
                new BreadcrumbPrefixItem
                {
                    Title = "Home",
                    Url = "/"
                }
            ]
        };

        // Act & Assert
        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => sut.GetStrategy(rule));

        Assert.Equal(
            "No breadcrumb strategy registered for key 'BaseJourney'.",
            exception.Message);
    }

    [Fact]
    public void GetStrategy_WhenIncrementalJourneyStrategyIsNotRegistered_ShouldThrowInvalidOperationException()
    {
        // Arrange
        ServiceProviderTestDouble serviceProvider =
            ServiceProviderTestDouble.Create();

        BreadcrumbStrategyFactory sut =
            new(serviceProvider);

        BreadcrumbRouteRule rule = new();

        // Act & Assert
        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => sut.GetStrategy(rule));

        Assert.Equal(
            "No breadcrumb strategy registered for key 'IncrementalJourney'.",
            exception.Message);
    }
}
