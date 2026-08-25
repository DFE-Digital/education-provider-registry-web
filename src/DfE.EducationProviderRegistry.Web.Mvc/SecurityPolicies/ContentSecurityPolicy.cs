namespace DfE.EducationProviderRegistry.Web.Mvc.SecurityPolicies;

internal static class ContentSecurityPolicy
{
    public const string Value =
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline' https://*.googletagmanager.com https://*.google-analytics.com https://*.analytics.google.com; " +
        "style-src 'self' 'unsafe-inline' https://*.googletagmanager.com https://fonts.googleapis.com; " +
        "img-src 'self' data: https://*.googletagmanager.com https://*.google-analytics.com https://*.analytics.google.com https://*.clarity.ms https://fonts.gstatic.com; " +
        "connect-src 'self' https://*.googletagmanager.com https://*.google-analytics.com https://*.analytics.google.com https://*.clarity.ms; " +
        "frame-src https://*.googletagmanager.com; " +
        "font-src 'self' data: https://fonts.gstatic.com; " +
        "object-src 'none'; " +
        "base-uri 'self'; " +
        "form-action 'self'; " +
        "frame-ancestors 'self';";
}