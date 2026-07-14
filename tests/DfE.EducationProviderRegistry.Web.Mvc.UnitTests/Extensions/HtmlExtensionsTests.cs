using DfE.EducationProviderRegistry.Web.Mvc.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Extensions
{

    public class HtmlExtensionsTests
    {
        [Fact]
        public void ActiveClassForPath_ReturnsActiveClass_WhenPathMatchesRoutePrefix()
        {
            // Arrange
            IHtmlHelper htmlHelper = CreateHtmlHelper("/search");

            // Act
            string result = htmlHelper.ActiveClassForPath("/search");

            // Assert
            Assert.Equal("govuk-service-navigation__item--active", result);
        }

        [Fact]
        public void ActiveClassForPath_ReturnsActiveClass_WhenPathStartsWithRoutePrefix()
        {
            // Arrange
            IHtmlHelper htmlHelper = CreateHtmlHelper("/search/results");

            // Act
            string result = htmlHelper.ActiveClassForPath("/search");

            // Assert
            Assert.Equal("govuk-service-navigation__item--active", result);
        }

        [Fact]
        public void ActiveClassForPath_ReturnsEmptyString_WhenPathDoesNotMatchRoutePrefix()
        {
            // Arrange
            IHtmlHelper htmlHelper = CreateHtmlHelper("/establishments");

            // Act
            string result = htmlHelper.ActiveClassForPath("/search");

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ActiveClassForPath_IsCaseInsensitive()
        {
            // Arrange
            IHtmlHelper htmlHelper = CreateHtmlHelper("/Search");

            // Act
            string result = htmlHelper.ActiveClassForPath("/SEARCH");

            // Assert
            Assert.Equal("govuk-service-navigation__item--active", result);
        }

        [Fact]
        public void ActiveClassForPath_ReturnsEmptyString_WhenPathIsNull()
        {
            // Arrange
            IHtmlHelper htmlHelper = CreateHtmlHelper(null);

            // Act
            string result = htmlHelper.ActiveClassForPath("/search");

            // Assert
            Assert.Equal(string.Empty, result);
        }

        private static IHtmlHelper CreateHtmlHelper(string? path)
        {
            DefaultHttpContext httpContext = new();

            if (path is not null)
                httpContext.Request.Path = path;

            ViewContext viewContext = new()
            {
                HttpContext = httpContext
            };

            Mock<IHtmlHelper> htmlHelperMock = new();
            htmlHelperMock.SetupGet(x => x.ViewContext).Returns(viewContext);

            return htmlHelperMock.Object;
        }
    }
}
