using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
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

    public BreadcrumbResultFilter(
        IBreadcrumbRouteResolver routeResolver,
        IBreadcrumbTitleResolver titleResolver,
        IBreadcrumbJourneyStore journeyStore,
        IBreadcrumbTracker tracker)
    {
        ArgumentNullException.ThrowIfNull(routeResolver);
        ArgumentNullException.ThrowIfNull(titleResolver);
        ArgumentNullException.ThrowIfNull(journeyStore);
        ArgumentNullException.ThrowIfNull(tracker);

        _routeResolver = routeResolver;
        _titleResolver = titleResolver;
        _journeyStore = journeyStore;
        _tracker = tracker;
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

        BreadcrumbItem currentBreadcrumbItem = new(title, currentUrl);

        _tracker.Track(journey, rule, currentBreadcrumbItem);

        _journeyStore.Save(journey);

        await next();
    }

    private void HandleResetMode(HttpContext context, BreadcrumbRouteRule rule)
    {
        if (rule.JourneyResetMode == BreadcrumbJourneyResetMode.Always ||
            (rule.JourneyResetMode == BreadcrumbJourneyResetMode.DirectNavigationOnly &&
             HttpContextNavigationHelpers.IsDirectNavigation(context)))
        {
            _journeyStore.Clear();
        }
    }
}
