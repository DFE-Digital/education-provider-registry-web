using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Filters.Breadcrumbs.Strategies;

public sealed class IncrementalJourneyStrategyTests
{
    [Fact]
    public void Constructor_WhenRulesIsNull_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new IncrementalJourneyStrategy(null!));
    }

    [Fact]
    public void Apply_WhenJourneyIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IncrementalJourneyStrategy strategy = new([]);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () => strategy.Apply(
                null!,
                new BreadcrumbRouteRule(),
                new BreadcrumbItem("Details", "/details")));
    }

    [Fact]
    public void Apply_WhenRuleIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IncrementalJourneyStrategy strategy = new([]);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () => strategy.Apply(
                new BreadcrumbJourney(),
                null!,
                new BreadcrumbItem("Details", "/details")));
    }

    [Fact]
    public void Apply_WhenCurrentBreadcrumbItemIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IncrementalJourneyStrategy strategy = new([]);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () => strategy.Apply(
                new BreadcrumbJourney(),
                new BreadcrumbRouteRule(),
                null!));
    }

    [Fact]
    public void Apply_WhenFirstRuleMatches_ShouldNotApplyFurtherRules()
    {
        // Arrange
        JourneyRuleTestDouble firstRule = new(true);
        JourneyRuleTestDouble secondRule = new(true);

        IncrementalJourneyStrategy strategy =
            new([firstRule, secondRule]);

        // Act
        strategy.Apply(
            new BreadcrumbJourney(),
            new BreadcrumbRouteRule(),
            new BreadcrumbItem("Details", "/details"));

        // Assert
        Assert.True(firstRule.ApplyCalled);
        Assert.False(secondRule.ApplyCalled);
    }

    [Fact]
    public void Apply_WhenNoRulesMatch_ShouldEvaluateAllRulesAndApplyNone()
    {
        // Arrange
        JourneyRuleTestDouble firstRule = new(false);
        JourneyRuleTestDouble secondRule = new(false);

        IncrementalJourneyStrategy strategy =
            new([firstRule, secondRule]);

        // Act
        strategy.Apply(
            new BreadcrumbJourney(),
            new BreadcrumbRouteRule(),
            new BreadcrumbItem("Details", "/details"));

        // Assert
        Assert.True(firstRule.IsSatisfiedByCalled);
        Assert.True(secondRule.IsSatisfiedByCalled);

        Assert.False(firstRule.ApplyCalled);
        Assert.False(secondRule.ApplyCalled);
    }

    [Fact]
    public void Apply_ShouldBuildJourneyContext()
    {
        // Arrange
        BreadcrumbJourney journey = new()
        {
            Items =
            [
                new BreadcrumbItem(
                    "Search Results",
                    "/search/results?query=test")
            ]
        };

        JourneyRuleTestDouble rule = new(true);

        IncrementalJourneyStrategy strategy =
            new([rule]);

        BreadcrumbItem currentItem =
            new("Establishment", "/establishments/123?source=search");

        // Act
        strategy.Apply(
            journey,
            new BreadcrumbRouteRule(),
            currentItem);

        // Assert
        Assert.NotNull(rule.ReceivedContext);

        Assert.Same(journey, rule.ReceivedContext.Journey);
        Assert.Same(currentItem, rule.ReceivedContext.Page);

        Assert.Equal(
            "/establishments/123",
            rule.ReceivedContext.CurrentPath);

        Assert.Equal(
            "/search/results",
            rule.ReceivedContext.LastPath);

        Assert.Equal(
            -1,
            rule.ReceivedContext.ExistingIndex);
    }

    [Fact]
    public void Apply_WhenJourneyIsEmpty_ShouldBuildExpectedContext()
    {
        // Arrange
        BreadcrumbJourney journey = new();

        JourneyRuleTestDouble rule = new(true);

        IncrementalJourneyStrategy strategy =
            new([rule]);

        BreadcrumbItem currentItem =
            new("Details", "/details");

        // Act
        strategy.Apply(
            journey,
            new BreadcrumbRouteRule(),
            currentItem);

        // Assert
        Assert.NotNull(rule.ReceivedContext);

        Assert.Equal(
            "/details",
            rule.ReceivedContext.CurrentPath);

        Assert.Equal(
            string.Empty,
            rule.ReceivedContext.LastPath);

        Assert.Equal(
            -1,
            rule.ReceivedContext.ExistingIndex);
    }

    [Fact]
    public void Apply_ShouldSetExistingIndex_WhenCurrentPathAlreadyExists()
    {
        // Arrange
        BreadcrumbJourney journey = new()
        {
            Items =
            [
                new BreadcrumbItem("Home", "/"),
                new BreadcrumbItem("Search", "/search"),
                new BreadcrumbItem("Details", "/establishments/123")
            ]
        };

        JourneyRuleTestDouble rule = new(true);

        IncrementalJourneyStrategy strategy =
            new([rule]);

        BreadcrumbItem currentItem =
            new("Details", "/establishments/123?id=1");

        // Act
        strategy.Apply(
            journey,
            new BreadcrumbRouteRule(),
            currentItem);

        // Assert
        Assert.NotNull(rule.ReceivedContext);

        Assert.Equal(
            2,
            rule.ReceivedContext.ExistingIndex);
    }

    [Fact]
    public void Apply_ShouldUseAbsolutePath_WhenCurrentUrlIsAbsolute()
    {
        // Arrange
        JourneyRuleTestDouble rule = new(true);

        IncrementalJourneyStrategy strategy =
            new([rule]);

        BreadcrumbItem currentItem =
            new(
                "Details",
                "https://test.com/establishments/123?foo=bar");

        // Act
        strategy.Apply(
            new BreadcrumbJourney(),
            new BreadcrumbRouteRule(),
            currentItem);

        // Assert
        Assert.NotNull(rule.ReceivedContext);

        Assert.Equal(
            "/establishments/123",
            rule.ReceivedContext.CurrentPath);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Apply_ShouldUseEmptyCurrentPath_WhenUrlIsNullOrWhitespace(string? url)
    {
        // Arrange
        JourneyRuleTestDouble rule = new(true);

        IncrementalJourneyStrategy strategy =
            new([rule]);

        BreadcrumbItem currentItem =
            new("Details", url!);

        // Act
        strategy.Apply(
            new BreadcrumbJourney(),
            new BreadcrumbRouteRule(),
            currentItem);

        // Assert
        Assert.NotNull(rule.ReceivedContext);

        Assert.Equal(
            string.Empty,
            rule.ReceivedContext.CurrentPath);
    }
}
