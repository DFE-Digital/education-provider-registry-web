using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Controllers;

public class CookiesController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SetPreferences(string cookies_analytics)
    {
        bool analyticsAccepted = cookies_analytics == "yes";

        Response.Cookies.Append(
            "cookies_analytics",
            analyticsAccepted ? "yes" : "no",
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                Secure = true,
                HttpOnly = false,
                SameSite = SameSiteMode.Strict
            });

        string referer = Request.Headers.Referer.ToString();
        if (!string.IsNullOrWhiteSpace(referer))
            return Redirect(referer);

        return RedirectToAction("Index", "Home");
    }
}
