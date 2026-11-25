using DfE.EducationProviderRegistry.Web.PageComponents.Commands;
using FluentValidation;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace DfE.EducationProviderRegistry.Web.SharedTests.PageComponents;

internal sealed class SendKeysCommandHandler
{
    private readonly WebDriverWait _wait;
    private readonly IValidator<SendKeysCommandModel> _validator;

    public SendKeysCommandHandler(IWebDriver driver, IValidator<SendKeysCommandModel> validator)
    {
        ArgumentNullException.ThrowIfNull(driver);
        ArgumentNullException.ThrowIfNull(validator);

        _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        _validator = validator;
    }

    public void Handle(SendKeysCommandModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        By by = By.CssSelector(model.Selector);

        _validator.ValidateAndThrow(model);

        _wait.Until(driver =>
        {
            try
            {
                IWebElement? element = driver
                    .FindElements(by)
                    .FirstOrDefault();

                if (element is null)
                {
                    return false;
                }

                element.SendKeys(model.Text);

                return true;
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
        });
    }
}

public sealed class SendKeysCommandModelValidator : AbstractValidator<SendKeysCommandModel>
{
    public SendKeysCommandModelValidator()
    {
        RuleFor(model => model.Selector)
            .NotEmpty();

        RuleFor(model => model.Text)
            .NotEmpty();
    }
}
