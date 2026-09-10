using DfE.EducationProviderRegistry.Web.Mvc.SecurityPolicies;

namespace DfE.EducationProviderRegistry.Web.Mvc.Middleware;

public static class SecurityHeadersMiddlewareExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            // Only report violations of the Content Security Policy, do not enforce it, so we can monitor for any issues before enforcing
            context.Response.Headers.ContentSecurityPolicyReportOnly =
                ContentSecurityPolicy.Value;
            await next();
        });
    }
}