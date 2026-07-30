using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests.Actions.Handlers;

internal sealed class SendKeysActionHandler : BaseAccessibilityScanActionHandler
{
    public override Task ExecuteAsync(AccessibilityScanContext context)
    {
        FindElement(context).SendKeys(context.Action.GetRequiredOption("text"));
        return Task.CompletedTask;
    }
}
