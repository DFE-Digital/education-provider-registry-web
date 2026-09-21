using Docker.DotNet.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;

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
        return _wait.Until((driver) => FindCheckboxes(driver));
    }

    // how do I find which checkboxes on the page to click (there may be multiple checkbox components - pass ISearchContext?
    public void Click(string text)
    {
        // TODO exception messages on timeout based on context?
        // _wait.Message = (labelThatMatches.Count == 0) or labelThatMatches > 0
        _wait.Until((driver) =>
        {
            IReadOnlyCollection<IWebElement> labelsThatMatches =
                [.. FindCheckboxes(driver)
                    .SelectMany((checkboxComponent) => checkboxComponent.FindElements(By.CssSelector(".govuk-checkboxes__label")))
                    .Where((label) => label.Text.Contains(text, StringComparison.OrdinalIgnoreCase))];

            if (labelsThatMatches.Count == 0)
            {
                return null;
            }

            if (labelsThatMatches.Count > 1)
            {
                return null;
            }

            labelsThatMatches.Single().Click();

            return labelsThatMatches;
        });
    }

    private static ReadOnlyCollection<IWebElement> FindCheckboxes(ISearchContext context) =>
            context.FindElements(
                By.CssSelector(".govuk-checkboxes"));
}