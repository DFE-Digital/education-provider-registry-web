using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Shared.Filters.TestDoubles;

public static class BreadcrumbOptionsTestDoubles
{
    public static IReadOnlyDictionary<string, string?> Stub()
    {
        BreadcrumbOptions options = Create(
        [
            BreadcrumbRouteRuleBuilder
                .Create("/search/results")
                .WithTitle("Search Results")
                .WithJourneyResetMode(BreadcrumbJourneyResetMode.DirectNavigationOnly)
                .WithPrefix("Home", "/")
                .WithPrefix("Search", "/search")
                .Build(),

            BreadcrumbRouteRuleBuilder
                .Create("/establishments")
                .WithTitleModelProperty("Heading")
                .WithJourneyResetMode(BreadcrumbJourneyResetMode.Always)
                .Build(),

            BreadcrumbRouteRuleBuilder
                .Create("/groups")
                .WithTitleModelProperty("Heading")
                .Build()
        ]);

        return BreadcrumbOptionsConfigurationAdapter.ToConfiguration(options);
    }

    public static BreadcrumbOptions Create(IEnumerable<BreadcrumbRouteRule> rules)
    {
        return new()
        {
            Rules = rules?.ToList() ?? []
        };
    }
}
