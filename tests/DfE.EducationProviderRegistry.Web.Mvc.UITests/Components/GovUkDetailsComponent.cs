using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;

namespace DfE.EducationProviderRegistry.Web.MVC.UITests.Components;

public sealed class GovUkDetailsComponent
{
    private readonly WebDriverWait _wait;
    public GovUkDetailsComponent(IWebDriver webDriver)
    {
        ArgumentNullException.ThrowIfNull(webDriver);
        _wait = new(webDriver, TimeSpan.FromSeconds(15));
    }

    public IReadOnlyCollection<IWebElement> Find()
    {
        return _wait.Until((driver) => FindDetails(driver));
    }

    public void Expand(string text)
    {
        _wait.Until((driver) =>
        {
            Func<IReadOnlyList<IWebElement>> matchingDetails =
                () =>
                    FindDetails(driver)
                        .Where((details) =>
                            details.FindElement(By.CssSelector(".govuk-details__summary-text")).Text
                                .Contains(text, StringComparison.OrdinalIgnoreCase))
                        .ToList();

            IReadOnlyList<IWebElement> locatedMatchingDetails = matchingDetails();

            // better errors than .Single()
            if (locatedMatchingDetails.Count == 0)
            {
                throw new ArgumentException($"Could not locate GovUkDetails with text {text}");
            }

            if (locatedMatchingDetails.Count > 1)
            {
                throw new InvalidOperationException($"Located multiple GovUkDetails with text {text}");
            }

            locatedMatchingDetails
                .Single()
                .FindElement(By.CssSelector(".govuk-details__summary-text"))
                .Click();

            // verify expanded target with "open" on details
            bool result = matchingDetails().Single().GetAttribute("open") is not null;

            return result;
        });
    }

    private static ReadOnlyCollection<IWebElement> FindDetails(IWebDriver driver) =>
        driver.FindElements(By.CssSelector(".govuk-details"));
}
