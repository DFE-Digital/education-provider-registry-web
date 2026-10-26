using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Shared.Filters.TestDoubles;

internal sealed class BreadcrumbOptionsBuilder
{
    List<BreadcrumbRouteRule> _rules;
    public BreadcrumbOptionsBuilder()
    {
        _rules = [];
    }

    public static BreadcrumbOptionsBuilder Create() => new();
    public BreadcrumbOptionsBuilder WithRule(string path, string? title = null, BreadcrumbJourneyResetMode? resetMode = null)
    {
        BreadcrumbRouteRule rule = new()
        {
            PathPattern = path,
            Title = title
        };

        if (resetMode is not null)
        {
            rule.JourneyResetMode = resetMode.Value;
        }

        _rules.Add(rule);

        return this;
    }

    public BreadcrumbOptions Build()
    {
        return new()
        {
            Rules = _rules
        };
    }
}
