using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.NavigationEvaluators;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Resolvers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Store;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Tracking;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs;

public sealed class BreadcrumbResultFilter : IAsyncResultFilter
{
    private readonly IBreadcrumbRouteResolver _routeResolver;
    private readonly IBreadcrumbTitleResolver _titleResolver;
    private readonly IBreadcrumbJourneyStore _journeyStore;
    private readonly IBreadcrumbTracker _tracker;
    private readonly INavigationEvaluator _navigationEvaluator;

    public BreadcrumbResultFilter(
        IBreadcrumbRouteResolver routeResolver,
        IBreadcrumbTitleResolver titleResolver,
        IBreadcrumbJourneyStore journeyStore,
        IBreadcrumbTracker tracker,
        INavigationEvaluator navigationEvaluator)
    {
        _routeResolver = routeResolver;
        _titleResolver = titleResolver;
        _journeyStore = journeyStore;
        _tracker = tracker;
        _navigationEvaluator = navigationEvaluator;
    }

    public async Task OnResultExecutionAsync(
        ResultExecutingContext context,
        ResultExecutionDelegate next)
    {
        if (context.Result is not ViewResult viewResult)
        {
            await next();
            return;
        }

        if (!_routeResolver.TryResolveRule(
            context.HttpContext.Request.Path,
            out BreadcrumbRouteRule? rule))
        {
            await next();
            return;
        }

        ArgumentNullException.ThrowIfNull(rule);
        HandleResetMode(context.HttpContext, rule);

        BreadcrumbJourney journey = _journeyStore.Load();
        string title = _titleResolver.Resolve(rule, viewResult);
        string currentUrl =
            $"{context.HttpContext.Request.Path}{context.HttpContext.Request.QueryString}";

        _tracker.Track(journey, rule, title, currentUrl);

        _journeyStore.Save(journey);

        await next();
    }

    private void HandleResetMode(HttpContext context, BreadcrumbRouteRule rule)
    {
        if (rule.JourneyResetMode == BreadcrumbJourneyResetMode.Always ||
            (rule.JourneyResetMode == BreadcrumbJourneyResetMode.DirectNavigationOnly &&
             _navigationEvaluator.IsDirectNavigation(context)))
        {
            _journeyStore.Clear();
        }
    }
}
