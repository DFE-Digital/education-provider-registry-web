using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace DfE.EducationProviderRegistry.Web.MVC.UITests.Extensions;

internal static class WebDriverWaitExtensions
{
    public static void Until(this WebDriverWait wait, Action<IWebDriver> action)
    {
        ArgumentNullException.ThrowIfNull(wait);
        ArgumentNullException.ThrowIfNull(action);

        wait.Until(driver =>
        {
            action(driver);
            return true;
        });
    }
}