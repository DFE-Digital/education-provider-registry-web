using DfE.EducationProviderRegistry.Web.Mvc.Extensions;
namespace DfE.EducationProviderRegistry.Web.Mvc.Settings;

public class GoogleAnalyticsSettings
{
    public string? ContainerId { get; set; }

    public string? AuthenticationId { get; set; }

    public string? PreviewValue { get; set; }

    public string Domain { get; } = "https://www.googletagmanager.com";

    public string TagManagerQueryString =>
        (!string.IsNullOrWhiteSpace(AuthenticationId) && !string.IsNullOrWhiteSpace(PreviewValue)
            ? $"&gtm_auth={Uri.EscapeDataString(AuthenticationId)}" +
              $"&gtm_preview={Uri.EscapeDataString(PreviewValue)}" +
              "&gtm_cookies_win=x"
            : string.Empty
        );

    public bool IsGoogleTagManagerEnabled(HttpContext context)
    {
        if (context == null) return false;

        return !string.IsNullOrWhiteSpace(ContainerId)
            && context.IsAnalyticsConsentGranted();
    }
}