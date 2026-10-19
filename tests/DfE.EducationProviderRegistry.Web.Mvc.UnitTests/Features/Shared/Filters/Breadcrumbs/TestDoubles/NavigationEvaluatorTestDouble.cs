using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.NavigationEvaluators;
using Microsoft.AspNetCore.Http;
using Moq;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Filters.Breadcrumbs.TestDoubles;

[ExcludeFromCodeCoverage]
public static class NavigationEvaluatorTestDouble
{
    public static Mock<INavigationEvaluator> Mock()
    {
        return new Mock<INavigationEvaluator>();
    }

    public static Mock<INavigationEvaluator> MockFor(
        bool isDirectNavigation)
    {
        Mock<INavigationEvaluator> mock = new();

        mock.Setup(evaluator =>
                evaluator.IsDirectNavigation(
                    It.IsAny<HttpContext>()))
            .Returns(isDirectNavigation);

        return mock;
    }
}
