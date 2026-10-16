using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies.JourneyRules;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies;

public sealed class IncrementalJourneyStrategy : IBreadcrumbStrategy
{
    private readonly IEnumerable<IJourneyRule> _rules;

    public IncrementalJourneyStrategy(IEnumerable<IJourneyRule> rules)
    {
        _rules = rules ??
            throw new ArgumentNullException(nameof(rules));
    }

    public void Apply(
        BreadcrumbJourney journey,
        BreadcrumbRouteRule rule,
        BreadcrumbItem currentBreadcrumbItem)
    {
        ArgumentNullException.ThrowIfNull(journey);
        ArgumentNullException.ThrowIfNull(rule);
        ArgumentNullException.ThrowIfNull(currentBreadcrumbItem);

        string currentPath = GetAbsolutePath(currentBreadcrumbItem.Url);
        string lastPath =
            journey.Items.Count > 0 ?
                GetAbsolutePath(journey.Items.Last().Url) : string.Empty;

        int existingIndex = journey.Items.FindIndex(
            breadcrumbItem =>
                GetAbsolutePath(breadcrumbItem.Url)
                    .Equals(currentPath, StringComparison.OrdinalIgnoreCase));

        JourneyContext context =
            new(journey, currentBreadcrumbItem, currentPath, lastPath, existingIndex);

        foreach (IJourneyRule journeyRule in _rules)
        {
            if (journeyRule.IsSatisfiedBy(context))
            {
                journeyRule.Apply(context);
                break;
            }
        }
    }

    private static string GetAbsolutePath(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return string.Empty;
        }

        // http://localhost is acting purely as a throwaway dummy base to satisfy the Uri constructor.
        Uri uri = new(new Uri("http://localhost"), url);
        return uri.AbsolutePath;
    }
}
