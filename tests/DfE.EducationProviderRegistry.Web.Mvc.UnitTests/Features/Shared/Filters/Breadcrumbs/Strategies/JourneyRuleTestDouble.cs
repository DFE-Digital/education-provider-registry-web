using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies.JourneyRules;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Filters.Breadcrumbs.Strategies;

public sealed class JourneyRuleTestDouble : IJourneyRule
{
    private readonly bool _isSatisfied;

    public JourneyContext? ReceivedContext { get; private set; }

    public bool IsSatisfiedByCalled { get; private set; }

    public bool ApplyCalled { get; private set; }

    public JourneyRuleTestDouble(bool isSatisfied)
    {
        _isSatisfied = isSatisfied;
    }

    public bool IsSatisfiedBy(JourneyContext context)
    {
        IsSatisfiedByCalled = true;
        ReceivedContext = context;

        return _isSatisfied;
    }

    public void Apply(JourneyContext context)
    {
        ApplyCalled = true;
        ReceivedContext = context;
    }
}
