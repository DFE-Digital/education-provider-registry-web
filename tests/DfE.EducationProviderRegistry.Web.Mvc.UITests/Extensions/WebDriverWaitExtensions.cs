using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace DfE.EducationProviderRegistry.Web.MVC.UITests.Extensions;

internal static class WebDriverWaitExtensions
{
    public static void Until(this IWait<IWebDriver> wait, Action<IWebDriver> action)
    {
        ArgumentNullException.ThrowIfNull(wait);
        ArgumentNullException.ThrowIfNull(action);

        wait.Until(driver =>
        {
            action(driver);
            return true;
        });
    }

    public static void Click(this IWait<IWebDriver> wait, By by)
    {
        IWebElement element = wait.Until(ElementClickable(by));

        element.Click();

        // Core Implementation from Selenium.Support
        static Func<IWebDriver, IWebElement?> ElementClickable(By by)
        {
            return driver =>
            {
                try
                {
                    IWebElement element = driver.FindElement(by);

                    return
                        ElementIfVisibleOrDefault(element) is not null &&
                        ElementIfEnabledOrDefault(element) is not null
                            ? element : null;
                }
                catch (StaleElementReferenceException)
                {
                    return null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
            };
        }
    }

    // Core Implementation from Selenium.Support
    public static Func<IWebDriver, IWebElement?> ElementToBeClickable(this IWebElement element)
    {
        return (driver) =>
        {
            try
            {
                if (ElementIfVisibleOrDefault(element) is not null && ElementIfEnabledOrDefault(element) is not null)
                {
                    return element;
                }
                else
                {
                    return null;
                }
            }
            catch (StaleElementReferenceException)
            {
                return null;
            }
        };
    }


    private static IWebElement? ElementIfVisibleOrDefault(IWebElement? element)
    {
        if (element is null)
        {
            return null;
        }

        return element.Displayed ? element : null;
    }

    private static IWebElement? ElementIfEnabledOrDefault(IWebElement? element)
    {
        if (element is null)
        {
            return null;
        }

        return element.Enabled ? element : null;
    }
}