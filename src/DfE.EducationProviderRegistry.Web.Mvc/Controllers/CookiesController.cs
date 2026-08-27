using DfE.EducationProviderRegistry.Web.Mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DfE.EducationProviderRegistry.Web.Mvc.Controllers;

public class CookiesController : Controller
{
    private const string CookieName = "cookies_policy";
    private const int CookieExpiryDays = 365;

    [HttpGet("/cookies")]
    public IActionResult Index([FromQuery] bool saved = false)
    {
        CookiesViewModel viewModel = new CookiesViewModel
        {
            Analytics = ReadAnalyticsConsent(),
            Saved = saved
        };

        return View(viewModel);
    }

    [HttpPost("/cookies")]
    [ValidateAntiForgeryToken]
    public IActionResult Save(bool? analytics)
    {
        bool accepted = analytics == true;

        CookieOptions options = new()
        {
            Expires = DateTimeOffset.UtcNow.AddDays(CookieExpiryDays),
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
            Secure = Request.IsHttps,
            Path = "/"
        };

        string json = JsonSerializer.Serialize(new { analytics = accepted });

        Response.Cookies.Append(CookieName, json, options);

        return RedirectToAction(nameof(Index), new { saved = true });
    }

    private bool? ReadAnalyticsConsent()
    {
        string? raw = Request.Cookies[CookieName];

        if (raw is null)
        {
            return null;
        }

        try
        {
            JsonDocument document = JsonDocument.Parse(raw);
            JsonElement root = document.RootElement;

            return root.GetProperty("analytics").GetBoolean();
        }
        catch
        {
            return null;
        }
    }
}
