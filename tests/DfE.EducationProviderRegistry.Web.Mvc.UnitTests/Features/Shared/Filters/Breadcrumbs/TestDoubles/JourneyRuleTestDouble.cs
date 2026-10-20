using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies.JourneyRules;
using Moq;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Filters.Breadcrumbs.TestDoubles;

[ExcludeFromCodeCoverage]
public static class JourneyRuleTestDouble
{
    public static Mock<IJourneyRule> Mock()
    {
        return new();
    }

    public static Mock<IJourneyRule> MockFor(
        bool isSatisfied)
    {
        Mock<IJourneyRule> mock = new();

        mock.Setup(rule =>
                rule.IsSatisfiedBy(
                    It.IsAny<JourneyContext>()))
            .Returns(isSatisfied);

        return mock;
    }
}
