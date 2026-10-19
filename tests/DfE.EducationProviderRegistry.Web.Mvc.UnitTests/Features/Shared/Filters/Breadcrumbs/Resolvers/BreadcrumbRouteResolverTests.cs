using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Resolvers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Filters.Breadcrumbs.Resolvers;

public sealed class BreadcrumbRouteResolverTests
{
    [Fact]
    public void Constructor_WhenOptionsIsNull_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new BreadcrumbRouteResolver(null!));
    }

    [Fact]
    public void TryResolveRule_WhenExactMatchExists_ShouldReturnTrue()
    {
        // Arrange
        BreadcrumbRouteRule expectedRule = new()
        {
            PathPattern = "/search"
        };

        BreadcrumbRouteResolver sut =
            CreateSut(expectedRule);

        // Act
        bool result =
            sut.TryResolveRule(
                new PathString("/search"),
                out BreadcrumbRouteRule? actualRule);

        // Assert
        Assert.True(result);
        Assert.Same(expectedRule, actualRule);
    }

    [Fact]
    public void TryResolveRule_WhenPrefixMatchExists_ShouldReturnTrue()
    {
        // Arrange
        BreadcrumbRouteRule expectedRule = new()
        {
            PathPattern = "/establishments"
        };

        BreadcrumbRouteResolver sut =
            CreateSut(expectedRule);

        // Act
        bool result =
            sut.TryResolveRule(
                new PathString("/establishments/123"),
                out BreadcrumbRouteRule? actualRule);

        // Assert
        Assert.True(result);
        Assert.Same(expectedRule, actualRule);
    }

    [Fact]
    public void TryResolveRule_WhenNoMatchExists_ShouldReturnFalse()
    {
        // Arrange
        BreadcrumbRouteResolver sut =
            CreateSut(
                new BreadcrumbRouteRule
                {
                    PathPattern = "/search"
                });

        // Act
        bool result =
            sut.TryResolveRule(
                new PathString("/groups/123"),
                out BreadcrumbRouteRule? actualRule);

        // Assert
        Assert.False(result);
        Assert.Null(actualRule);
    }

    [Fact]
    public void TryResolveRule_WhenMultipleRulesMatch_ShouldReturnMostSpecificRule()
    {
        // Arrange
        BreadcrumbRouteRule lessSpecific = new()
        {
            PathPattern = "/datasets"
        };

        BreadcrumbRouteRule moreSpecific = new()
        {
            PathPattern = "/datasets/complete"
        };

        BreadcrumbRouteResolver sut =
            CreateSut(
                lessSpecific,
                moreSpecific);

        // Act
        bool result =
            sut.TryResolveRule(
                new PathString("/datasets/complete"),
                out BreadcrumbRouteRule? actualRule);

        // Assert
        Assert.True(result);
        Assert.Same(moreSpecific, actualRule);
    }

    [Fact]
    public void TryResolveRule_WhenCaseDiffers_ShouldMatchIgnoringCase()
    {
        // Arrange
        BreadcrumbRouteRule expectedRule = new()
        {
            PathPattern = "/search"
        };

        BreadcrumbRouteResolver sut =
            CreateSut(expectedRule);

        // Act
        bool result =
            sut.TryResolveRule(
                new PathString("/SEARCH"),
                out BreadcrumbRouteRule? actualRule);

        // Assert
        Assert.True(result);
        Assert.Same(expectedRule, actualRule);
    }

    [Fact]
    public void TryResolveRule_WhenPathValueIsNull_ShouldReturnFalse()
    {
        // Arrange
        BreadcrumbRouteResolver sut =
            CreateSut(
                new BreadcrumbRouteRule
                {
                    PathPattern = "/search"
                });

        // Act
        bool result =
            sut.TryResolveRule(
                default,
                out BreadcrumbRouteRule? actualRule);

        // Assert
        Assert.False(result);
        Assert.Null(actualRule);
    }

    [Fact]
    public void TryResolveRule_WhenPathOnlyPartiallyMatches_ShouldReturnFalse()
    {
        // Arrange
        BreadcrumbRouteResolver sut =
            CreateSut(
                new BreadcrumbRouteRule
                {
                    PathPattern = "/search"
                });

        // Act
        bool result =
            sut.TryResolveRule(
                new PathString("/searchresults"),
                out BreadcrumbRouteRule? actualRule);

        // Assert
        Assert.False(result);
        Assert.Null(actualRule);
    }

    private static BreadcrumbRouteResolver CreateSut(
        params BreadcrumbRouteRule[] rules)
    {
        BreadcrumbOptions options = new()
        {
            Rules = [.. rules]
        };

        return new BreadcrumbRouteResolver(
            Options.Create(options));
    }
}
