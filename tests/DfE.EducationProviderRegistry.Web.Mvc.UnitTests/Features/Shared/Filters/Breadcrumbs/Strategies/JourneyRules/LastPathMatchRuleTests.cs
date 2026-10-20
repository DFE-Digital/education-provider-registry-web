using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies.JourneyRules;
using DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Filters.Breadcrumbs.TestDoubles;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Filters.Breadcrumbs.Strategies.JourneyRules;

public sealed class LastPathMatchRuleTests
{
    private readonly LastPathMatchRule _sut = new();

    [Fact]
    public void IsSatisfiedBy_WhenJourneyContainsItemsAndPathsMatch_ShouldReturnTrue()
    {
        // Arrange
        JourneyContext context =
            JourneyContextTestDouble.CreateWithJourney(
                new BreadcrumbItem(
                    "School A",
                    "/establishments/123"));

        context = JourneyContextTestDouble.Create(
            journey: context.Journey,
            currentPath: "/establishments/123",
            lastPath: "/establishments/123");

        // Act
        bool result = _sut.IsSatisfiedBy(context);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsSatisfiedBy_WhenJourneyIsEmpty_ShouldReturnFalse()
    {
        // Arrange
        JourneyContext context =
            JourneyContextTestDouble.Create(
                currentPath: "/establishments/123",
                lastPath: "/establishments/123");

        // Act
        bool result = _sut.IsSatisfiedBy(context);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsSatisfiedBy_WhenPathsDoNotMatch_ShouldReturnFalse()
    {
        // Arrange
        JourneyContext context =
            JourneyContextTestDouble.CreateWithJourney(
                new BreadcrumbItem(
                    "School A",
                    "/establishments/123"));

        context = JourneyContextTestDouble.Create(
            journey: context.Journey,
            currentPath: "/groups/456",
            lastPath: "/establishments/123");

        // Act
        bool result = _sut.IsSatisfiedBy(context);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsSatisfiedBy_WhenPathsMatchIgnoringCase_ShouldReturnTrue()
    {
        // Arrange
        JourneyContext context =
            JourneyContextTestDouble.CreateWithJourney(
                new BreadcrumbItem(
                    "School A",
                    "/establishments/123"));

        context = JourneyContextTestDouble.Create(
            journey: context.Journey,
            currentPath: "/ESTABLISHMENTS/123",
            lastPath: "/establishments/123");

        // Act
        bool result = _sut.IsSatisfiedBy(context);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Apply_ShouldReplaceLastJourneyItemWithCurrentPage()
    {
        // Arrange
        BreadcrumbItem search =
            new("Search Results", "/search/results");

        BreadcrumbItem oldSchool =
            new("Old School", "/establishments/123");

        BreadcrumbItem page =
            new("New School", "/establishments/123");

        JourneyContext context =
            JourneyContextTestDouble.Create(
                journey: new BreadcrumbJourney
                {
                    Items =
                    [
                        search,
                        oldSchool
                    ]
                },
                page: page);

        // Act
        _sut.Apply(context);

        // Assert
        Assert.Equal(2, context.Journey.Items.Count);
        Assert.Equal(search, context.Journey.Items[0]);
        Assert.Equal(page, context.Journey.Items[1]);
    }
}
