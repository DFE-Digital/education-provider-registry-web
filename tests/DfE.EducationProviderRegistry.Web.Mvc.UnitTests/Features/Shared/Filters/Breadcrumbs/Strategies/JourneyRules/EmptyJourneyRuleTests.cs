using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies.JourneyRules;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Filters.Breadcrumbs.Strategies.JourneyRules;

public sealed class EmptyJourneyRuleTests
{
    private readonly EmptyJourneyRule _sut = new();

    [Fact]
    public void IsSatisfiedBy_WhenJourneyIsEmpty_ShouldReturnTrue()
    {
        // Arrange
        JourneyContext context =
            JourneyContextTestDouble.CreateEmpty();

        // Act
        bool result = _sut.IsSatisfiedBy(context);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsSatisfiedBy_WhenJourneyContainsItems_ShouldReturnFalse()
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
    public void Apply_ShouldAddPageToJourney()
    {
        // Arrange
        BreadcrumbItem page =
            new("Establishment", "/establishments/123");

        JourneyContext context =
            JourneyContextTestDouble.Create(
                page: page);

        // Act
        _sut.Apply(context);

        // Assert
        Assert.Single(context.Journey.Items);
        Assert.Equal(page, context.Journey.Items[0]);
    }
}
