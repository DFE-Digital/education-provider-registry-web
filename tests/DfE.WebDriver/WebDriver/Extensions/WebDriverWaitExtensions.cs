using OpenQA.Selenium;

namespace DfE.WebDriver.WebDriver.Extensions;

public static class WebDriverWaitExtensions
{
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
