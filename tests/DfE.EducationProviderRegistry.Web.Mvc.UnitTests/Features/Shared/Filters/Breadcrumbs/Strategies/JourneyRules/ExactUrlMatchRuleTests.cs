using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies.JourneyRules;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Filters.Breadcrumbs.Strategies.JourneyRules;

public sealed class ExactUrlMatchRuleTests
{
    private readonly ExactUrlMatchRule _sut = new();

    [Fact]
    public void IsSatisfiedBy_WhenJourneyIsEmpty_ShouldReturnFalse()
    {
        // Arrange
        JourneyContext context =
            JourneyContextTestDouble.CreateEmpty();

        // Act
        bool result = _sut.IsSatisfiedBy(context);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsSatisfiedBy_WhenLastJourneyItemUrlMatchesPageUrl_ShouldReturnTrue()
    {
        // Arrange
        BreadcrumbItem page =
            new("Establishment", "/establishments/123");

        JourneyContext context =
            JourneyContextTestDouble.CreateWithPageAndJourney(
                page,
                new BreadcrumbItem(
                    "Establishment",
                    "/establishments/123"));

        // Act
        bool result = _sut.IsSatisfiedBy(context);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsSatisfiedBy_WhenLastJourneyItemUrlDoesNotMatchPageUrl_ShouldReturnFalse()
    {
        // Arrange
        JourneyContext context =
            JourneyContextTestDouble.CreateWithPageAndJourney(
                new BreadcrumbItem(
                    "Establishment",
                    "/establishments/123"),
                new BreadcrumbItem(
                    "Search Results",
                    "/search/results"));

        // Act
        bool result = _sut.IsSatisfiedBy(context);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsSatisfiedBy_ShouldIgnoreUrlCase()
    {
        // Arrange
        JourneyContext context =
            JourneyContextTestDouble.CreateWithPageAndJourney(
                new BreadcrumbItem(
                    "Establishment",
                    "/establishments/123"),
                new BreadcrumbItem(
                    "Establishment",
                    "/ESTABLISHMENTS/123"));

        // Act
        bool result = _sut.IsSatisfiedBy(context);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Apply_ShouldDoNothing()
    {
        // Arrange
        JourneyContext context =
            JourneyContextTestDouble.CreateWithPageAndJourney(
                new BreadcrumbItem(
                    "Establishment",
                    "/establishments/123"),
                new BreadcrumbItem(
                    "Search Results",
                    "/search/results"));

        List<BreadcrumbItem> originalItems =
            [.. context.Journey.Items];

        // Act
        _sut.Apply(context);

        // Assert
        Assert.Equal(
            originalItems,
            context.Journey.Items);
    }
}
