namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies.JourneyRules;

public sealed class DefaultFallbackRule : IJourneyRule
{
    public bool IsSatisfiedBy(JourneyContext context) => true;
    public void Apply(JourneyContext context) =>
        context.Journey.Items.Add(context.Page);
}
