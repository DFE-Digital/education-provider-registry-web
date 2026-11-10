using DfE.EducationProviderRegistry.Web.PageComponents.Commands;
using FluentValidation;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace DfE.EducationProviderRegistry.Web.SharedTests.PageComponents;

internal sealed class ClickCommandModelValidator : AbstractValidator<ClickCommandModel>
{
    public ClickCommandModelValidator()
    {
        RuleFor(model => model.Selector)
            .NotEmpty();
    }
}

internal sealed class ClickCommandHandler
{
    private readonly WebDriverWait _wait;
    private readonly IValidator<ClickCommandModel> _validator;

    public ClickCommandHandler(IWebDriver driver, IValidator<ClickCommandModel> validator)
    {
        ArgumentNullException.ThrowIfNull(driver);
        ArgumentNullException.ThrowIfNull(validator);

        _wait = new WebDriverWait(
            driver,
            TimeSpan.FromSeconds(15));

        _validator = validator;
    }

    public void Handle(ClickCommandModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        _validator.ValidateAndThrow(model);

        By by = By.CssSelector(model.Selector);

        _wait.Until(driver =>
        {
            try
            {
                IEnumerable<IWebElement> elements = driver.FindElements(by);

                if (!string.IsNullOrWhiteSpace(model.WithText))
                {
                    elements = elements.Where(element =>
                        element.Text.Contains(
                            model.WithText,
                            StringComparison.OrdinalIgnoreCase));
                }

                IReadOnlyCollection<IWebElement> matches = [.. elements];

                if (matches.Count != 1)
                {
                    return false;
                }

                matches.Single().Click();

                return true;
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
            catch (ElementClickInterceptedException)
            {
                return false;
            }
        });
    }
}
