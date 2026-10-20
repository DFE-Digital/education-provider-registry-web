using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies.JourneyRules;
using DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Filters.Breadcrumbs.TestDoubles;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Filters.Breadcrumbs.Strategies.JourneyRules;

public sealed class ExistingPathMatchRuleTests
{
    private readonly ExistingPathMatchRule _sut = new();

    [Fact]
    public void IsSatisfiedBy_WhenExistingIndexIsGreaterThanOrEqualToZero_ShouldReturnTrue()
    {
        // Arrange
        JourneyContext context =
            JourneyContextTestDouble.Create(
                existingIndex: 0);

        // Act
        bool result = _sut.IsSatisfiedBy(context);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsSatisfiedBy_WhenExistingIndexIsNegative_ShouldReturnFalse()
    {
        // Arrange
        JourneyContext context =
            JourneyContextTestDouble.Create(
                existingIndex: -1);

        // Act
        bool result = _sut.IsSatisfiedBy(context);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Apply_ShouldTruncateJourneyAndReplaceExistingItem()
    {
        // Arrange
        BreadcrumbItem search =
            new("Search", "/search");

        BreadcrumbItem results =
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
                        results,
                        oldSchool
                    ]
                },
                page: page,
                existingIndex: 2);

        // Act
        _sut.Apply(context);

        // Assert
        Assert.Equal(3, context.Journey.Items.Count);
        Assert.Equal(search, context.Journey.Items[0]);
        Assert.Equal(results, context.Journey.Items[1]);
        Assert.Equal(page, context.Journey.Items[2]);
    }

    [Fact]
    public void Apply_ShouldRemoveItemsAfterExistingIndex()
    {
        // Arrange
        BreadcrumbItem page =
            new("School A", "/establishments/123");

        JourneyContext context =
            JourneyContextTestDouble.Create(
                journey: new BreadcrumbJourney
                {
                    Items =
                    [
                        new BreadcrumbItem("Search", "/search"),
                        new BreadcrumbItem("Results", "/search/results"),
                        new BreadcrumbItem("School A", "/establishments/123"),
                        new BreadcrumbItem("Group", "/groups/456")
                    ]
                },
                page: page,
                existingIndex: 2);

        // Act
        _sut.Apply(context);

        // Assert
        Assert.Equal(3, context.Journey.Items.Count);
        Assert.DoesNotContain(
            context.Journey.Items,
            x => x.Title == "Group");
    }
}
