using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Resolvers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Filters.Breadcrumbs.TestDoubles;

[ExcludeFromCodeCoverage]
public static class BreadcrumbTitleResolverTestDouble
{
    public static Mock<IBreadcrumbTitleResolver> Mock()
    {
        return new Mock<IBreadcrumbTitleResolver>();
    }

    public static Mock<IBreadcrumbTitleResolver> MockFor(
        string title)
    {
        Mock<IBreadcrumbTitleResolver> mock = new();

        mock.Setup(resolver =>
                resolver.Resolve(
                    It.IsAny<BreadcrumbRouteRule>(),
                    It.IsAny<ViewResult>()))
            .Returns(title);

        return mock;
    }
}
