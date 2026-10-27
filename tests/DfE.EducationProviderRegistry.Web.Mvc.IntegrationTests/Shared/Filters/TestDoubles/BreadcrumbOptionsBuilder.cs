using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Shared.Filters.TestDoubles;

public sealed class BreadcrumbRouteRuleBuilder
{
    private readonly string _pathPattern;
    private string? _title;
    private string? _titleModelProperty;
    private BreadcrumbJourneyResetMode? _journeyResetMode;
    private readonly List<BreadcrumbPrefixItem> _baseJourney = [];

    private BreadcrumbRouteRuleBuilder(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _pathPattern = path;
    }

    public static BreadcrumbRouteRuleBuilder Create(string pathPattern)
    {
        return new(pathPattern);
    }

    public BreadcrumbRouteRuleBuilder WithTitle(string? title)
    {
        _title = title;
        return this;
    }

    public BreadcrumbRouteRuleBuilder WithTitleModelProperty(string? titleModelProperty)
    {
        _titleModelProperty = titleModelProperty;
        return this;
    }

    public BreadcrumbRouteRuleBuilder WithJourneyResetMode(
        BreadcrumbJourneyResetMode journeyResetMode)
    {
        _journeyResetMode = journeyResetMode;
        return this;
    }

    public BreadcrumbRouteRuleBuilder WithPrefix(string title, string url)
    {
        _baseJourney.Add(new BreadcrumbPrefixItem
        {
            Title = title,
            Url = url
        });

        return this;
    }

    public BreadcrumbRouteRule Build()
    {
        return new BreadcrumbRouteRule
        {
            PathPattern = _pathPattern,
            Title = _title,
            TitleModelProperty = _titleModelProperty,
            JourneyResetMode = _journeyResetMode is null ? default : _journeyResetMode.Value,
            BaseJourney = [.. _baseJourney]
        };
    }
}
