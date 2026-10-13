using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Middleware.Breadcrumbs.Options;

namespace DfE.EducationProviderRegistry.Web.Mvc.Middleware.Breadcrumbs.Strategies;

public sealed class IncrementalJourneyStrategy : IBreadcrumbStrategy
{
    public void Apply(BreadcrumbJourney journey, BreadcrumbRouteRule rule, string currentUrl)
    {
        BreadcrumbItem page = new(rule.Title, currentUrl);

        if (journey.Items.Count is 0)
        {
            journey.Items.Add(page);
            return;
        }

        if (journey.Items.Last().Url.Equals(page.Url, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        string currentPath = GetAbsolutePath(currentUrl);
        string lastPath = GetAbsolutePath(journey.Items.Last().Url);

        if (lastPath.Equals(currentPath, StringComparison.OrdinalIgnoreCase))
        {
            journey.Items[journey.Items.Count - 1] = page;
            return;
        }

        int existingIndex =
            journey.Items.FindIndex(
                breadcrumbItem =>
                    GetAbsolutePath(breadcrumbItem.Url)
                        .Equals(currentPath, StringComparison.OrdinalIgnoreCase));

        if (existingIndex >= 0)
        {
            journey.Items = [.. journey.Items.Take(existingIndex + 1)];
            journey.Items[existingIndex] = page;
            return;
        }

        journey.Items.Add(page);
    }

    private static string GetAbsolutePath(string url)
    {
        Uri uri = new(new Uri("http://localhost"), url);

        return string.IsNullOrWhiteSpace(url) ?
            string.Empty : uri.AbsolutePath;
    }
}
