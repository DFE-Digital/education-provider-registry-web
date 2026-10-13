using DfE.EducationProviderRegistry.Web.Mvc.Middleware.Breadcrumbs.Options;
using Microsoft.Extensions.Options;

namespace DfE.EducationProviderRegistry.Web.Mvc.Middleware.Breadcrumbs.Routing;

public sealed class BreadcrumbRouteResolver : IBreadcrumbRouteResolver
{
    private readonly BreadcrumbOptions _options;

    public BreadcrumbRouteResolver(IOptions<BreadcrumbOptions> options)
    {
        _options = options?.Value ??
            throw new ArgumentNullException(nameof(options));
    }

    public bool TryResolveRule(PathString path, out BreadcrumbRouteRule? rule)
    {
        string pathValue = path.Value ?? string.Empty;

        rule =
            _options.Rules
                .OrderByDescending(breadcrumbRouteRule =>
                    breadcrumbRouteRule.PathPattern.Length)
                .FirstOrDefault(breadcrumbRouteRule =>
                    MatchesPath(pathValue, breadcrumbRouteRule.PathPattern));

        return rule != null;
    }

    private static bool MatchesPath(string requestPath, string pattern) =>
        requestPath.Equals(pattern, StringComparison.OrdinalIgnoreCase) ||
        requestPath.StartsWith(pattern + "/", StringComparison.OrdinalIgnoreCase);
}
