using System.Text.Json;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Resolvers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs;

public sealed class BreadcrumbResultFilter : IAsyncResultFilter
{
    private const string SessionKey = "BreadcrumbJourney";

    private readonly IBreadcrumbRouteResolver _routeResolver;
    private readonly IBreadcrumbTitleResolver _titleResolver;

    public BreadcrumbResultFilter(
        IBreadcrumbRouteResolver routeResolver,
        IBreadcrumbTitleResolver titleResolver)
    {
        ArgumentNullException.ThrowIfNull(routeResolver);
        ArgumentNullException.ThrowIfNull(titleResolver);
        _routeResolver = routeResolver;
        _titleResolver = titleResolver;
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

        ISession session = context.HttpContext.Session;

        switch (rule.JourneyResetMode)
        {
            case BreadcrumbJourneyResetMode.Always:

                session.Remove(SessionKey);

                await next();

                return;

            case BreadcrumbJourneyResetMode.DirectNavigationOnly
                when IsDirectNavigation(context.HttpContext):

                session.Remove(SessionKey);
                break;
        }

        BreadcrumbJourney journey = LoadJourney(session);

        string title = _titleResolver.Resolve(rule, viewResult);

        Track(
            journey,
            rule,
            title,
            $"{context.HttpContext.Request.Path}{context.HttpContext.Request.QueryString}");

        SaveJourney(
            session,
            journey);

        await next();
    }

    private static BreadcrumbJourney LoadJourney(ISession session)
    {
        string? json = session.GetString(SessionKey);

        return string.IsNullOrWhiteSpace(json)
            ? new BreadcrumbJourney()
            : JsonSerializer.Deserialize<BreadcrumbJourney>(json)
              ?? new BreadcrumbJourney();
    }

    private static void SaveJourney(ISession session, BreadcrumbJourney journey)
    {
        session.SetString(SessionKey, JsonSerializer.Serialize(journey));
    }

    private static bool IsDirectNavigation(HttpContext context)
    {
        string? referer =
            context.Request.Headers.Referer;

        if (string.IsNullOrWhiteSpace(referer))
            return true;

        if (!Uri.TryCreate(
                referer,
                UriKind.Absolute,
                out Uri? refererUri))
        {
            return true;
        }

        return !refererUri.Host.Equals(
            context.Request.Host.Host,
            StringComparison.OrdinalIgnoreCase);
    }

    private static void Track(
        BreadcrumbJourney journey,
        BreadcrumbRouteRule rule,
        string title,
        string currentUrl)
    {
        BreadcrumbItem current =
            new(title, currentUrl);

        if (rule.BaseJourney.Count > 0)
        {
            ApplyBaseJourney(
                journey,
                rule,
                current);
        }
        else
        {
            ApplyIncrementalJourney(
                journey,
                current);
        }
    }

    private static void ApplyBaseJourney(
        BreadcrumbJourney journey,
        BreadcrumbRouteRule rule,
        BreadcrumbItem current)
    {
        journey.Items =
        [
            .. rule.BaseJourney.Select(
                item => new BreadcrumbItem(
                    item.Title,
                    item.Url))
        ];

        journey.Items.Add(current);
    }

    private static void ApplyIncrementalJourney(
        BreadcrumbJourney journey,
        BreadcrumbItem current)
    {
        if (journey.Items.Count == 0)
        {
            journey.Items.Add(current);
            return;
        }

        string currentPath =
            GetPath(current.Url);

        int existingIndex =
            journey.Items.FindIndex(
                item =>
                    GetPath(item.Url)
                        .Equals(
                            currentPath,
                            StringComparison.OrdinalIgnoreCase));

        if (existingIndex >= 0)
        {
            journey.Items =
            [
                .. journey.Items.Take(existingIndex + 1)
            ];

            journey.Items[existingIndex] = current;

            return;
        }

        journey.Items.Add(current);
    }

    private static string GetPath(string url)
    {
        return url.Split('?')[0];
    }
}
