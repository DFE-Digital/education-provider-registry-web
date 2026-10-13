using System.Text.Json;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Middleware.Breadcrumbs.Options;
using DfE.EducationProviderRegistry.Web.Mvc.Middleware.Breadcrumbs.Routing;
using DfE.EducationProviderRegistry.Web.Mvc.Middleware.Breadcrumbs.Strategies;
using DfE.EducationProviderRegistry.Web.Mvc.Middleware.Breadcrumbs.Strategies.Factories;
using Microsoft.AspNetCore.Http.Extensions;

namespace DfE.EducationProviderRegistry.Web.Mvc.Middleware.Breadcrumbs;

public sealed class BreadcrumbTrackingMiddleware
{
    private const string SessionKey = "BreadcrumbJourney";
    private readonly RequestDelegate _next;

    public BreadcrumbTrackingMiddleware(RequestDelegate next)
    {
        _next = next ??
            throw new ArgumentNullException(nameof(next));
    }

    public async Task InvokeAsync(
        HttpContext context,
        IBreadcrumbRouteResolver routeResolver,
        IBreadcrumbStrategyFactory strategyFactory)
    {
        if (ShouldTrackRequest(context) &&
            routeResolver.TryResolveRule(context.Request.Path, out BreadcrumbRouteRule? rule))
        {
            BreadcrumbJourney journey = LoadJourney(context.Session);
            string currentUrl = context.Request.GetDisplayUrl();
            IBreadcrumbStrategy strategy = strategyFactory.GetStrategy(rule!);
            strategy.Apply(journey, rule!, currentUrl);
            SaveJourney(context.Session, journey);
            const string HttpContextItemKey = "CurrentBreadcrumbJourney";
            context.Items[HttpContextItemKey] = journey;
        }

        await _next(context);
    }

    private static bool ShouldTrackRequest(HttpContext context)
    {
        if (!HttpMethods.IsGet(context.Request.Method))
            return false;

        string path =
            context.Request.Path.Value ?? string.Empty;

        return !path.StartsWith("/css", StringComparison.OrdinalIgnoreCase) &&
               !path.StartsWith("/js", StringComparison.OrdinalIgnoreCase) &&
               !path.StartsWith("/lib", StringComparison.OrdinalIgnoreCase) &&
               !path.StartsWith("/api", StringComparison.OrdinalIgnoreCase) &&
               !path.Contains('.');
    }

    private static BreadcrumbJourney LoadJourney(ISession session)
    {
        string? json = session.GetString(SessionKey);

        return (string.IsNullOrWhiteSpace(json)) ?
            new BreadcrumbJourney() :
            JsonSerializer.Deserialize<BreadcrumbJourney>(json) ?? new BreadcrumbJourney();
    }

    private static void SaveJourney(ISession session, BreadcrumbJourney journey)
    {
        string json = JsonSerializer.Serialize(journey);
        session.SetString(SessionKey, json);
    }
}
