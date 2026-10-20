using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies.Factories;
using Moq;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Filters.Breadcrumbs.TestDoubles;

[ExcludeFromCodeCoverage]
public static class BreadcrumbStrategyFactoryTestDouble
{
    public static Mock<IBreadcrumbStrategyFactory> Mock()
    {
        return new();
    }

    public static Mock<IBreadcrumbStrategyFactory> MockFor(
        IBreadcrumbStrategy strategy)
    {
        Mock<IBreadcrumbStrategyFactory> mock = new();

        mock.Setup(factory =>
                factory.GetStrategy(
                    It.IsAny<BreadcrumbRouteRule>()))
            .Returns(strategy);

        return mock;
    }
}

public static class BreadcrumbStrategyTestDouble
{
    public static Mock<IBreadcrumbStrategy> Mock()
    {
        return new();
    }
}
