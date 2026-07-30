namespace DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests.Actions.Handlers;

internal sealed class ClickActionHandler : BaseAccessibilityScanActionHandler
{
    public override Task ExecuteAsync(AccessibilityScanContext context)
    {
        FindElement(context).Click();
        return Task.CompletedTask;
    }
}
