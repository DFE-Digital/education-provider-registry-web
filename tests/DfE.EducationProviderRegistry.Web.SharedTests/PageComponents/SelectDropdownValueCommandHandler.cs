using DfE.EducationProviderRegistry.Web.PageComponents.Commands;
using FluentValidation;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace DfE.EducationProviderRegistry.Web.SharedTests.PageComponents;

internal sealed class SelectDropdownValueCommandModelValidator : AbstractValidator<SelectDropdownValueCommandModel>
{
    public SelectDropdownValueCommandModelValidator()
    {
        RuleFor(model => model.Selector)
            .NotEmpty();

        RuleFor(model => model.Value)
            .NotEmpty();
    }
}

internal sealed class SelectDropdownValueCommandHandler
{
    private readonly WebDriverWait _wait;
    private readonly IValidator<SelectDropdownValueCommandModel> _validator;

    public SelectDropdownValueCommandHandler(IWebDriver driver, IValidator<SelectDropdownValueCommandModel> validator)
    {
        ArgumentNullException.ThrowIfNull(driver);
        ArgumentNullException.ThrowIfNull(validator);

        _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        _validator = validator;
    }

    public void Handle(SelectDropdownValueCommandModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        _validator.ValidateAndThrow(model);

        By by = By.CssSelector(model.Selector);

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

                SelectElement selectElement = new(element);

                selectElement.SelectByValue(model.Value);

                return true;
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
        });
    }
}
