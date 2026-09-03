using System.Text.Json;

namespace DfE.EducationProviderRegistry.Web.Mvc.Extensions;

/// <summary>
/// Reads the user's analytics consent from the <c>cookies_policy</c> cookie.
/// Returns true only when the cookie exists, is valid JSON, and contains
/// an "analytics": true property. Any missing, malformed, or unexpected
/// cookie shape is treated as no consent (fail closed).
/// </summary>
public static class AnalyticsConsentExtensions
{
    /// <summary>
    /// Returns whether the user has granted analytics consent based on the
    /// <c>cookies_policy</c> request cookie.
    /// </summary>
    public static bool IsAnalyticsConsentGranted(this HttpContext context)
    {
        const string CookieName = "cookies_policy";

        if (!context.Request.Cookies.TryGetValue(
            CookieName, out string? rawCookie) ||
            string.IsNullOrWhiteSpace(rawCookie))
        {
            return false;
        }

        rawCookie = Uri.UnescapeDataString(rawCookie);

        if (!TryParseAnalyticsFlag(rawCookie, out bool analytics))
        {
            return false;
        }

        return analytics;
    }

    /// <summary>
    /// Attempts to parse the JSON cookie and extract the "analytics" boolean.
    /// Returns false if JSON is malformed, the property is missing, or the value
    /// is not a boolean. This ensures strict fail-closed behaviour.
    /// </summary>
    private static bool TryParseAnalyticsFlag(string json, out bool value)
    {
        value = false;

        try
        {
            JsonDocument document = JsonDocument.Parse(json);

            JsonElement root = document.RootElement;

            // Try to get the "analytics" property; fail closed if missing.
            if (!root.TryGetProperty("analytics", out JsonElement analyticsElement))
            {
                return true; // JSON was valid, but no consent.
            }

            // Only treat explicit true as consent.
            if (analyticsElement.ValueKind == JsonValueKind.True)
            {
                value = true;
            }

            return true;
        }
        catch
        {
            // Malformed JSON so fail closed.
            return false;
        }
    }
}

