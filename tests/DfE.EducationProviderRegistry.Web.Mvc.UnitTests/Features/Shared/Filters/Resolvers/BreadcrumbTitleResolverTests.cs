using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Resolvers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Filters.Resolvers;

public sealed class BreadcrumbTitleResolverTests
{
    private readonly BreadcrumbTitleResolver _sut = new();

    [Fact]
    public void Resolve_WhenTitleIsSpecified_ShouldReturnTitle()
    {
        // Arrange
        BreadcrumbRouteRule rule = new()
        {
            Title = "Search Results"
        };

        ViewResult viewResult = new();

        // Act
        string result = _sut.Resolve(
            rule,
            viewResult);

        // Assert
        Assert.Equal(
            "Search Results",
            result);
    }

    [Fact]
    public void Resolve_WhenTitleModelPropertyIsSpecified_ShouldReturnPropertyValue()
    {
        // Arrange
        BreadcrumbRouteRule rule = new()
        {
            TitleModelProperty = "Heading"
        };

        ViewResult viewResult = new()
        {
            ViewData = new ViewDataDictionary(
                new EmptyModelMetadataProvider(),
                new ModelStateDictionary())
            {
                Model = new TestViewModel
                {
                    Heading = "St Mary's Primary School"
                }
            }
        };

        // Act
        string result = _sut.Resolve(
            rule,
            viewResult);

        // Assert
        Assert.Equal(
            "St Mary's Primary School",
            result);
    }

    [Fact]
    public void Resolve_WhenModelPropertyDoesNotExist_ShouldReturnEmptyString()
    {
        // Arrange
        BreadcrumbRouteRule rule = new()
        {
            TitleModelProperty = "DoesNotExist"
        };

        ViewResult viewResult = new()
        {
            ViewData = new ViewDataDictionary(
                new EmptyModelMetadataProvider(),
                new ModelStateDictionary())
            {
                Model = new TestViewModel
                {
                    Heading = "St Mary's Primary School"
                }
            }
        };

        // Act
        string result = _sut.Resolve(
            rule,
            viewResult);

        // Assert
        Assert.Equal(
            string.Empty,
            result);
    }

    [Fact]
    public void Resolve_WhenModelPropertyValueIsNull_ShouldReturnEmptyString()
    {
        // Arrange
        BreadcrumbRouteRule rule = new()
        {
            TitleModelProperty = "Heading"
        };

        ViewResult viewResult = new()
        {
            ViewData = new ViewDataDictionary(
                new EmptyModelMetadataProvider(),
                new ModelStateDictionary())
            {
                Model = new TestViewModel()
            }
        };

        // Act
        string result = _sut.Resolve(
            rule,
            viewResult);

        // Assert
        Assert.Equal(
            string.Empty,
            result);
    }

    [Fact]
    public void Resolve_WhenNeitherTitleAndTitleModelPropertyIsSpecified_ShouldReturnEmptyString()
    {
        // Arrange
        BreadcrumbRouteRule rule = new();

        ViewResult viewResult = new();

        // Act
        string result = _sut.Resolve(
            rule,
            viewResult);

        // Assert
        Assert.Equal(
            string.Empty,
            result);
    }


    [Fact]
    public void Resolve_WhenModelIsNull_ShouldReturnEmptyString()
    {
        // Arrange
        BreadcrumbRouteRule rule = new()
        {
            TitleModelProperty = "Heading"
        };

        ViewResult viewResult = new();

        // Act
        string result = _sut.Resolve(
            rule,
            viewResult);

        // Assert
        Assert.Equal(
            string.Empty,
            result);
    }

    private sealed class TestViewModel
    {
        public string? Heading { get; init; }
    }
}
