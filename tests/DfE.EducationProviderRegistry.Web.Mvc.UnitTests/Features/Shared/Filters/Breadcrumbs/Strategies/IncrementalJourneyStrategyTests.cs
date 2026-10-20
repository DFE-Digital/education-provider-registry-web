using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies.JourneyRules;
using DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Filters.Breadcrumbs.TestDoubles;
using Moq;

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
        Mock<IJourneyRule> firstRule =
            JourneyRuleTestDouble.MockFor(true);

        Mock<IJourneyRule> secondRule =
            JourneyRuleTestDouble.MockFor(true);

        IncrementalJourneyStrategy strategy =
            new([firstRule.Object, secondRule.Object]);

        // Act
        strategy.Apply(
            new BreadcrumbJourney(),
            new BreadcrumbRouteRule(),
            new BreadcrumbItem("Details", "/details"));

        // Assert
        firstRule.Verify(
            rule => rule.Apply(
                It.IsAny<JourneyContext>()),
            Times.Once);

        secondRule.Verify(
            rule => rule.Apply(
                It.IsAny<JourneyContext>()),
            Times.Never);
    }

    [Fact]
    public void Apply_WhenNoRulesMatch_ShouldEvaluateAllRulesAndApplyNone()
    {
        // Arrange
        Mock<IJourneyRule> firstRule =
            JourneyRuleTestDouble.MockFor(false);

        Mock<IJourneyRule> secondRule =
            JourneyRuleTestDouble.MockFor(false);

        IncrementalJourneyStrategy strategy =
            new([firstRule.Object, secondRule.Object]);

        // Act
        strategy.Apply(
            new BreadcrumbJourney(),
            new BreadcrumbRouteRule(),
            new BreadcrumbItem("Details", "/details"));

        // Assert
        firstRule.Verify(
            rule => rule.IsSatisfiedBy(
                It.IsAny<JourneyContext>()),
            Times.Once);

        secondRule.Verify(
            rule => rule.IsSatisfiedBy(
                It.IsAny<JourneyContext>()),
            Times.Once);

        firstRule.Verify(
            rule => rule.Apply(
                It.IsAny<JourneyContext>()),
            Times.Never);

        secondRule.Verify(
            rule => rule.Apply(
                It.IsAny<JourneyContext>()),
            Times.Never);
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

        JourneyContext? receivedContext = null;

        Mock<IJourneyRule> rule =
            JourneyRuleTestDouble.MockFor(true);

        rule.Setup(r =>
                r.IsSatisfiedBy(
                    It.IsAny<JourneyContext>()))
            .Callback<JourneyContext>(
                context => receivedContext = context)
            .Returns(true);

        IncrementalJourneyStrategy strategy =
            new([rule.Object]);

        BreadcrumbItem currentItem =
            new(
                "Establishment",
                "/establishments/123?source=search");

        // Act
        strategy.Apply(
            journey,
            new BreadcrumbRouteRule(),
            currentItem);

        // Assert
        Assert.NotNull(receivedContext);

        Assert.Same(journey, receivedContext!.Journey);
        Assert.Same(currentItem, receivedContext.Page);

        Assert.Equal(
            "/establishments/123",
            receivedContext.CurrentPath);

        Assert.Equal(
            "/search/results",
            receivedContext.LastPath);

        Assert.Equal(
            -1,
            receivedContext.ExistingIndex);
    }

    [Fact]
    public void Apply_WhenJourneyIsEmpty_ShouldBuildExpectedContext()
    {
        // Arrange
        JourneyContext? receivedContext = null;

        Mock<IJourneyRule> rule =
            JourneyRuleTestDouble.MockFor(true);

        rule.Setup(r =>
                r.IsSatisfiedBy(
                    It.IsAny<JourneyContext>()))
            .Callback<JourneyContext>(
                context => receivedContext = context)
            .Returns(true);

        IncrementalJourneyStrategy strategy =
            new([rule.Object]);

        // Act
        strategy.Apply(
            new BreadcrumbJourney(),
            new BreadcrumbRouteRule(),
            new BreadcrumbItem("Details", "/details"));

        // Assert
        Assert.NotNull(receivedContext);

        Assert.Equal(
            "/details",
            receivedContext!.CurrentPath);

        Assert.Equal(
            string.Empty,
            receivedContext.LastPath);

        Assert.Equal(
            -1,
            receivedContext.ExistingIndex);
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

        JourneyContext? receivedContext = null;

        Mock<IJourneyRule> rule =
            JourneyRuleTestDouble.MockFor(true);

        rule.Setup(r =>
                r.IsSatisfiedBy(
                    It.IsAny<JourneyContext>()))
            .Callback<JourneyContext>(
                context => receivedContext = context)
            .Returns(true);

        IncrementalJourneyStrategy strategy =
            new([rule.Object]);

        // Act
        strategy.Apply(
            journey,
            new BreadcrumbRouteRule(),
            new BreadcrumbItem(
                "Details",
                "/establishments/123?id=1"));

        // Assert
        Assert.NotNull(receivedContext);

        Assert.Equal(
            2,
            receivedContext!.ExistingIndex);
    }

    [Fact]
    public void Apply_ShouldUseAbsolutePath_WhenCurrentUrlIsAbsolute()
    {
        // Arrange
        JourneyContext? receivedContext = null;

        Mock<IJourneyRule> rule =
            JourneyRuleTestDouble.MockFor(true);

        rule.Setup(r =>
                r.IsSatisfiedBy(
                    It.IsAny<JourneyContext>()))
            .Callback<JourneyContext>(
                context => receivedContext = context)
            .Returns(true);

        IncrementalJourneyStrategy strategy =
            new([rule.Object]);

        // Act
        strategy.Apply(
            new BreadcrumbJourney(),
            new BreadcrumbRouteRule(),
            new BreadcrumbItem(
                "Details",
                "https://test.com/establishments/123?foo=bar"));

        // Assert
        Assert.NotNull(receivedContext);

        Assert.Equal(
            "/establishments/123",
            receivedContext!.CurrentPath);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Apply_ShouldUseEmptyCurrentPath_WhenUrlIsNullOrWhitespace(
    string? url)
    {
        // Arrange
        JourneyContext? receivedContext = null;

        Mock<IJourneyRule> rule =
            JourneyRuleTestDouble.MockFor(true);

        rule.Setup(r =>
                r.IsSatisfiedBy(
                    It.IsAny<JourneyContext>()))
            .Callback<JourneyContext>(
                context => receivedContext = context)
            .Returns(true);

        IncrementalJourneyStrategy strategy =
            new([rule.Object]);

        // Act
        strategy.Apply(
            new BreadcrumbJourney(),
            new BreadcrumbRouteRule(),
            new BreadcrumbItem(
                "Details",
                url!));

        // Assert
        Assert.NotNull(receivedContext);

        Assert.Equal(
            string.Empty,
            receivedContext!.CurrentPath);
    }
}
