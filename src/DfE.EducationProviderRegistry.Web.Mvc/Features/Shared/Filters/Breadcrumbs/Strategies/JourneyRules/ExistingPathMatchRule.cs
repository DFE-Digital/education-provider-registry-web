namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies.JourneyRules;

public sealed class ExistingPathMatchRule : IJourneyRule
{
    public bool IsSatisfiedBy(JourneyContext context) => context.ExistingIndex >= 0;
    public void Apply(JourneyContext context)
    {
        context.Journey.Items =
            [
                .. context.Journey.Items.Take(context.ExistingIndex + 1)
            ];

        context.Journey.Items[context.ExistingIndex] = context.Page;
    }
}
