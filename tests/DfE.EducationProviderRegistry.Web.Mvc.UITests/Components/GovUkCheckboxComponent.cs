using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace DfE.EducationProviderRegistry.Web.MVC.UITests.Components;

internal sealed class GovUkCheckboxComponent
{
    private readonly WebDriverWait _wait;
    public GovUkCheckboxComponent(IWebDriver driver)
    {
        _wait = new(driver, TimeSpan.FromSeconds(15));
    }
    // TODO don't rich model e.g. Checkbox (label, value, map yet....
    // all checkbox components returned - Client forced to filter for which checkbox component they want on page and map
    public IReadOnlyCollection<IWebElement> Find()
    {
        return _wait.Until((driver) =>
            driver.FindElements(
                By.CssSelector(".govuk-checkboxes")));
    }

    // how do I find which checkbox I want to click
    public void Click(string text)
    {
        _wait.Until((driver) =>
        {
            IReadOnlyCollection<IWebElement> all = Find();

            IReadOnlyCollection<IWebElement> labelsThatMatches =
                all.Select((checkboxComponent) => checkboxComponent.FindElement(By.CssSelector(".govuk-checkboxes__label")))
                    .Where((label) => label.Text.Contains(text, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            if (labelsThatMatches.Count == 0)
            {
                throw new InvalidOperationException($"No checkboxes found with label text {text}");
            }

            if (labelsThatMatches.Count > 1)
            {
                throw new ArgumentException($"Multiple click targets available with text {text}");
            }

            labelsThatMatches.Single().Click();
        });
    }
}