namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies.JourneyRules;

public interface IJourneyRule
{
    bool IsSatisfiedBy(JourneyContext context);
    void Apply(JourneyContext context);
}
