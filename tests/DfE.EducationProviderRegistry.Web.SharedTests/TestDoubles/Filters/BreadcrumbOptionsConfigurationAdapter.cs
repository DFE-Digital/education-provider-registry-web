using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;

namespace DfE.EducationProviderRegistry.Web.SharedTests.TestDoubles.Filters;

public static class BreadcrumbOptionsConfigurationAdapter
{
    public static IReadOnlyDictionary<string, string?> ToConfiguration(BreadcrumbOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        Dictionary<string, string?> configuration = [];

        for (int ruleIndex = 0; ruleIndex < options.Rules.Count; ruleIndex++)
        {
            BreadcrumbRouteRule rule = options.Rules[ruleIndex];
            string ruleKey = $"Breadcrumbs:{nameof(BreadcrumbOptions.Rules)}:{ruleIndex}";

            configuration[$"{ruleKey}:{nameof(BreadcrumbRouteRule.PathPattern)}"] = rule.PathPattern;
            configuration[$"{ruleKey}:{nameof(BreadcrumbRouteRule.Title)}"] = rule.Title;
            configuration[$"{ruleKey}:{nameof(BreadcrumbRouteRule.TitleModelProperty)}"] = rule.TitleModelProperty;
            configuration[$"{ruleKey}:{nameof(BreadcrumbRouteRule.JourneyResetMode)}"] = rule.JourneyResetMode.ToString();

            for (int journeyIndex = 0; journeyIndex < rule.BaseJourney.Count; journeyIndex++)
            {
                BreadcrumbPrefixItem item = rule.BaseJourney[journeyIndex];
                string journeyKey = $"{ruleKey}:{nameof(BreadcrumbRouteRule.BaseJourney)}:{journeyIndex}";

                configuration[$"{journeyKey}:{nameof(BreadcrumbPrefixItem.Title)}"] = item.Title;
                configuration[$"{journeyKey}:{nameof(BreadcrumbPrefixItem.Url)}"] = item.Url;
            }
        }

        return configuration;
    }
}
