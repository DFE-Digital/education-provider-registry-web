using DfE.EducationProviderRegistry.Web.Mvc.Controllers;
using DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Controllers.TestDoubles;
using DfE.EducationProviderRegistry.Web.Mvc.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Controllers;

public sealed class CookiesControllerTests
{
    private const string CookieName = "cookies_policy";

    [Fact]
    public void Index_ReturnsView_WithCorrectViewModel_WhenSavedFalse()
    {
        // arrange
        DefaultHttpContext httpContext = new();
        CookiesController controller = CreateController(httpContext);

        // act
        IActionResult result = controller.Index(saved: false);
        
        // assert
        ViewResult view = Assert.IsType<ViewResult>(result);
        CookiesViewModel model = Assert.IsType<CookiesViewModel>(view.Model);
        Assert.Null(model.Analytics);
        Assert.False(model.Saved);
    }

    [Fact]
    public void Index_ReturnsView_WithCorrectViewModel_WhenSavedTrue()
    {
        // arrange
        DefaultHttpContext httpContext = new();
        CookiesController controller = CreateController(httpContext);

        // act
        IActionResult result = controller.Index(saved: true);

        // assert
        ViewResult view = Assert.IsType<ViewResult>(result);
        CookiesViewModel model = Assert.IsType<CookiesViewModel>(view.Model);
        Assert.Null(model.Analytics);
        Assert.True(model.Saved);
    }

    [Fact]
    public void Save_SetsCookie_WithAnalyticsTrue_WhenAnalyticsIsTrue()
    {
        // arrange
        DefaultHttpContext httpContext = new();
        CookiesController controller = CreateController(httpContext);

        // act
        IActionResult result = controller.Save(true);
        string header = httpContext.Response.Headers.SetCookie.ToString();
        string json = CookieHeaderParserTestDouble.ExtractJson(header);
        JsonDocument doc = JsonDocument.Parse(json);

        // assert
        Assert.IsType<RedirectToActionResult>(result);
        Assert.False(string.IsNullOrEmpty(header));
        Assert.True(doc.RootElement.GetProperty("analytics").GetBoolean());
    }

    [Fact]
    public void Save_SetsCookie_WithAnalyticsFalse_WhenAnalyticsIsNull()
    {
        // arrange
        DefaultHttpContext httpContext = new();
        CookiesController controller = CreateController(httpContext);

        // act
        IActionResult result = controller.Save(null);
        string header = httpContext.Response.Headers.SetCookie.ToString();
        string json = CookieHeaderParserTestDouble.ExtractJson(header);
        JsonDocument doc = JsonDocument.Parse(json);

        // assert
        Assert.IsType<RedirectToActionResult>(result);
        Assert.False(string.IsNullOrEmpty(header));
        Assert.False(doc.RootElement.GetProperty("analytics").GetBoolean());
    }

    [Fact]
    public void ReadAnalyticsConsent_ReturnsNull_WhenCookieMissing()
    {
        // arrange
        DefaultHttpContext httpContext = new();
        CookiesController controller = CreateController(httpContext);

        // act
        bool? result =
            CookiesControllerInvokerTestDouble
                .InvokeReadAnalyticsConsent(controller);

        // assert
        Assert.Null(result);
    }

    [Fact]
    public void ReadAnalyticsConsent_ReturnsNull_WhenCookieMalformed()
    {
        // arrange
        DefaultHttpContext httpContext = new();
        httpContext.Request.Cookies = new RequestCookieCollectionStub
        {
            { CookieName, Uri.EscapeDataString("{not valid json") }
        };

        CookiesController controller = CreateController(httpContext);

        // act
        bool? result =
            CookiesControllerInvokerTestDouble
                .InvokeReadAnalyticsConsent(controller);

        // assert
        Assert.Null(result);
    }

    [Fact]
    public void ReadAnalyticsConsent_ReturnsNull_WhenAnalyticsPropertyMissing()
    {
        // arrange
        DefaultHttpContext httpContext = new();
        httpContext.Request.Cookies = new RequestCookieCollectionStub
        {
            { CookieName, Uri.EscapeDataString("{\"somethingElse\": true}" )}
        };

        CookiesController controller = CreateController(httpContext);

        // act
        bool? result =
            CookiesControllerInvokerTestDouble
                .InvokeReadAnalyticsConsent(controller);

        // assert
        Assert.Null(result);
    }

    [Fact]
    public void ReadAnalyticsConsent_ReturnsTrue_WhenAnalyticsTrue()
    {
        // arrange
        DefaultHttpContext httpContext = new();
        httpContext.Request.Cookies = new RequestCookieCollectionStub
        {
            { CookieName, Uri.EscapeDataString("{\"analytics\": true}") }
        };

        CookiesController controller = CreateController(httpContext);

        // act
        bool? result =
            CookiesControllerInvokerTestDouble
                .InvokeReadAnalyticsConsent(controller);

        //assert
        Assert.True(result);
    }

    [Fact]
    public void ReadAnalyticsConsent_ReturnsFalse_WhenAnalyticsFalse()
    {
        // arrange
        DefaultHttpContext httpContext = new();
        httpContext.Request.Cookies = new RequestCookieCollectionStub
        {
            { CookieName, Uri.EscapeDataString("{\"analytics\": false}") }
        };

        CookiesController controller = CreateController(httpContext);

        // act
        bool? result =
            CookiesControllerInvokerTestDouble
                .InvokeReadAnalyticsConsent(controller);

        // assert
        Assert.False(result);
    }

    private static CookiesController CreateController(DefaultHttpContext httpContext)
    {
        return new CookiesController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };
    }
}
