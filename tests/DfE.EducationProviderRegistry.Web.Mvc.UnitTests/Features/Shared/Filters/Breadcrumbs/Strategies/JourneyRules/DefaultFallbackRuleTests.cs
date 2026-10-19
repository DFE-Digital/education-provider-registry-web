using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies.JourneyRules;
using DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Filters.Breadcrumbs.TestDoubles;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Filters.Breadcrumbs.Strategies.JourneyRules;

public sealed class DefaultFallbackRuleTests
{
    private readonly DefaultFallbackRule _sut = new();

    [Fact]
    public void IsSatisfiedBy_ShouldReturnTrue()
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
    public void Apply_ShouldAddPageToJourney()
    {
        // Arrange
        BreadcrumbItem page =
            new("Establishment", "/establishments/123");

        JourneyContext context =
            JourneyContextTestDouble.Create(
                page: page,
                currentPath: "/establishments/123");

        // Act
        _sut.Apply(context);

        // Assert
        Assert.Single(context.Journey.Items);
        Assert.Equal(page, context.Journey.Items[0]);
    }
}
