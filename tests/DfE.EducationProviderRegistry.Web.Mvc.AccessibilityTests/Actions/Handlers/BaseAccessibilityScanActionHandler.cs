using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests.Actions.Handlers;

internal abstract class BaseAccessibilityScanActionHandler
    : IAccessibilityScanActionHandler
{
    public abstract Task ExecuteAsync(
        AccessibilityScanContext context);

    // TODO DefaultWebDriverOptions and back into ActionOptions
    protected IWebElement FindElement(AccessibilityScanContext context)
    {
        WebDriverWait wait = new(
            context.WebDriver,
            TimeSpan.FromSeconds(15));

        return wait.Until(
            driver =>
                driver.FindElement(
                    By.CssSelector(
                        context.Action.GetRequiredOption("target"))));
    }
}