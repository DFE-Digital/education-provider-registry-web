using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared;
using Microsoft.AspNetCore.Http;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Shared;

public sealed class HttpContextNavigationHelpersTests
{
    [Fact]
    public void IsDirectNavigation_WhenRefererIsMissing_ShouldReturnTrue()
    {
        // Arrange
        DefaultHttpContext context = new();

        context.Request.Host = new HostString(
            "education-provider.service.gov.uk");

        // Act
        bool result =
            HttpContextNavigationHelpers.IsDirectNavigation(context);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsDirectNavigation_WhenRefererIsInvalid_ShouldReturnTrue()
    {
        // Arrange
        DefaultHttpContext context = new();

        context.Request.Host = new HostString(
            "education-provider.service.gov.uk");

        context.Request.Headers.Referer =
            "not-a-valid-uri";

        // Act
        bool result =
            HttpContextNavigationHelpers.IsDirectNavigation(context);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsDirectNavigation_WhenRefererHostDiffers_ShouldReturnTrue()
    {
        // Arrange
        DefaultHttpContext context = new();

        context.Request.Host = new HostString(
            "education-provider.service.gov.uk");

        context.Request.Headers.Referer =
            "https://www.google.com/search";

        // Act
        bool result =
            HttpContextNavigationHelpers.IsDirectNavigation(context);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsDirectNavigation_WhenRefererHostMatches_ShouldReturnFalse()
    {
        // Arrange
        DefaultHttpContext context = new();

        context.Request.Host = new HostString(
            "education-provider.service.gov.uk");

        context.Request.Headers.Referer =
            "https://education-provider.service.gov.uk/search";

        // Act
        bool result =
            HttpContextNavigationHelpers.IsDirectNavigation(context);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsDirectNavigation_WhenRefererHostDiffersOnlyByCase_ShouldReturnFalse()
    {
        // Arrange
        DefaultHttpContext context = new();

        context.Request.Host = new HostString(
            "education-provider.service.gov.uk");

        context.Request.Headers.Referer =
            "https://EDUCATION-PROVIDER.SERVICE.GOV.UK/search";

        // Act
        bool result =
            HttpContextNavigationHelpers.IsDirectNavigation(context);

        // Assert
        Assert.False(result);
    }
}
