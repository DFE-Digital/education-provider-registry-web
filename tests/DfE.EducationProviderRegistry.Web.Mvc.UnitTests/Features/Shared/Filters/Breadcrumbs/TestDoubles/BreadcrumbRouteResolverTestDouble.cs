using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Resolvers;
using Microsoft.AspNetCore.Http;
using Moq;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Filters.Breadcrumbs.TestDoubles;

[ExcludeFromCodeCoverage]
public static class BreadcrumbRouteResolverTestDouble
{
    public static Mock<IBreadcrumbRouteResolver> Mock()
    {
        return new Mock<IBreadcrumbRouteResolver>();
    }

    public static Mock<IBreadcrumbRouteResolver> MockFor(
        BreadcrumbRouteRule rule)
    {
        Mock<IBreadcrumbRouteResolver> mock = new();

        BreadcrumbRouteRule? resolvedRule = rule;

        mock.Setup(resolver =>
                resolver.TryResolveRule(
                    It.IsAny<PathString>(),
                    out resolvedRule))
            .Returns(true);

        return mock;
    }

    public static Mock<IBreadcrumbRouteResolver> MockNoMatch()
    {
        Mock<IBreadcrumbRouteResolver> mock = new();

        BreadcrumbRouteRule? resolvedRule = null;

        mock.Setup(resolver =>
                resolver.TryResolveRule(
                    It.IsAny<PathString>(),
                    out resolvedRule))
            .Returns(false);

        return mock;
    }
}
