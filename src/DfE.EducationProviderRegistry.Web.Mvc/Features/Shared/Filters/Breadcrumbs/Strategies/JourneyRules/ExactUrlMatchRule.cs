namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies.JourneyRules;

public sealed class ExactUrlMatchRule : IJourneyRule
{
    public bool IsSatisfiedBy(JourneyContext context) =>
        context.Journey.Items.Count > 0 &&
        context.Journey.Items.Last()
            .Url.Equals(context.Page.Url, StringComparison.OrdinalIgnoreCase);

    public void Apply(JourneyContext context) { } // Do nothing in this case.
}
