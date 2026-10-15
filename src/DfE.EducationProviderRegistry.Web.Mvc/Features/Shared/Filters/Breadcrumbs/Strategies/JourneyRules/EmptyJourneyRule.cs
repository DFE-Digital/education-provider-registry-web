namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies.JourneyRules;

public sealed class EmptyJourneyRule : IJourneyRule
{
    public bool IsSatisfiedBy(JourneyContext context) =>
        context.Journey.Items.Count is 0;

    public void Apply(JourneyContext context) =>
        context.Journey.Items.Add(context.Page);
}
