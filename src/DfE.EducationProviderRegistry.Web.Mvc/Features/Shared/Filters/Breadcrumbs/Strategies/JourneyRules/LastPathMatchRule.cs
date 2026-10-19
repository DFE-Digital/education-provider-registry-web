namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies.JourneyRules;

public sealed class LastPathMatchRule : IJourneyRule
{
    public bool IsSatisfiedBy(JourneyContext context) =>
        context.Journey.Items.Count > 0 &&
        context.LastPath.Equals(
            context.CurrentPath,
            StringComparison.OrdinalIgnoreCase);

    public void Apply(JourneyContext context) =>
        context.Journey.Items[context.Journey.Items.Count - 1] = context.Page;
}
