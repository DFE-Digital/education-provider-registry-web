using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Filters.Breadcrumbs.Strategies;

public sealed class BaseJourneyStrategyTests
{
    private readonly BaseJourneyStrategy _strategy = new();

    [Fact]
    public void Apply_WhenBaseJourneyExists_ShouldPopulateJourneyAndAppendCurrentItem()
    {
        // Arrange
        BreadcrumbJourney journey = new();

        BreadcrumbRouteRule rule = new()
        {
            BaseJourney =
            [
                new BreadcrumbPrefixItem() { Title = "Home", Url = "/" },
                new BreadcrumbPrefixItem() { Title = "Search", Url = "/search" }
            ]
        };

        BreadcrumbItem current = new("Details", "/details");

        // Act
        _strategy.Apply(journey, rule, current);

        // Assert
        Assert.Collection(
            journey.Items,
            item =>
            {
                Assert.Equal("Home", item.Title);
                Assert.Equal("/", item.Url);
            },
            item =>
            {
                Assert.Equal("Search", item.Title);
                Assert.Equal("/search", item.Url);
            },
            item =>
            {
                Assert.Equal("Details", item.Title);
                Assert.Equal("/details", item.Url);
            });
    }

    [Fact]
    public void Apply_WhenBaseJourneyIsNull_ShouldOnlyAddCurrentItem()
    {
        // Arrange
        BreadcrumbJourney journey = new();

        BreadcrumbRouteRule rule = new()
        {
            BaseJourney = null!
        };

        BreadcrumbItem current = new("Details", "/details");

        // Act
        _strategy.Apply(journey, rule, current);

        // Assert
        Assert.Single(journey.Items);

        BreadcrumbItem item = journey.Items.Single();

        Assert.Equal("Details", item.Title);
        Assert.Equal("/details", item.Url);
    }

    [Fact]
    public void Apply_ShouldReplaceExistingJourneyItems()
    {
        // Arrange
        BreadcrumbJourney journey = new()
        {
            Items =
            [
                new BreadcrumbItem("Old", "/old")
            ]
        };

        BreadcrumbRouteRule rule = new()
        {
            BaseJourney =
            [
                new BreadcrumbPrefixItem()
                {
                    Title = "Home",
                    Url = "/"
                }
            ]
        };

        BreadcrumbItem current = new("Details", "/details");

        // Act
        _strategy.Apply(journey, rule, current);

        // Assert
        Assert.Equal(2, journey.Items.Count);

        Assert.DoesNotContain(
            journey.Items,
            item => item.Title == "Old");
    }

    [Fact]
    public void Apply_ShouldCreateNewBreadcrumbInstancesForBaseJourneyItems()
    {
        // Arrange
        BreadcrumbPrefixItem baseItem = new()
        {
            Title = "Home",
            Url = "/"
        };

        BreadcrumbJourney journey = new();

        BreadcrumbRouteRule rule = new()
        {
            BaseJourney = [baseItem]
        };

        BreadcrumbItem current = new("Details", "/details");

        // Act
        _strategy.Apply(journey, rule, current);

        // Assert
        Assert.Single(journey.Items.Take(1));

        Assert.Equal(baseItem.Title, journey.Items[0].Title);
        Assert.Equal(baseItem.Url, journey.Items[0].Url);
    }

    [Fact]
    public void Apply_WhenJourneyIsNull_ShouldThrowArgumentNullException()
    {
        BreadcrumbRouteRule rule = new();
        BreadcrumbItem current = new("Details", "/details");

        Assert.Throws<ArgumentNullException>(
            () => _strategy.Apply(null!, rule, current));
    }

    [Fact]
    public void Apply_WhenRuleIsNull_ShouldThrowArgumentNullException()
    {
        BreadcrumbJourney journey = new();
        BreadcrumbItem current = new("Details", "/details");

        Assert.Throws<ArgumentNullException>(
            () => _strategy.Apply(journey, null!, current));
    }

    [Fact]
    public void Apply_WhenCurrentBreadcrumbItemIsNull_ShouldThrowArgumentNullException()
    {
        BreadcrumbJourney journey = new();
        BreadcrumbRouteRule rule = new();

        Assert.Throws<ArgumentNullException>(
            () => _strategy.Apply(journey, rule, null!));
    }
}
