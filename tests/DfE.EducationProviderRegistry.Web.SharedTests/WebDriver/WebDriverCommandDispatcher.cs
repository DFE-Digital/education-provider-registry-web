using DfE.Document.Abstractions;
using DfE.EducationProviderRegistry.Web.PageComponents.Commands;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace DfE.EducationProviderRegistry.Web.SharedTests.WebDriver;

// Temporary in dispatcher until lifecycle retrieval and composition are linked.
public sealed class WebDriverCommandDispatcher : ICommandDispatcher
{
    private readonly IWebDriver _driver;

    public WebDriverCommandDispatcher(IWebDriver driver)
    {
        ArgumentNullException.ThrowIfNull(driver);

        _driver = driver;
    }

    public void Dispatch<TCommand, TCommandModel>(TCommandModel model)
        where TCommand : ICommand<TCommandModel>
        where TCommandModel : notnull
    {
        if (typeof(TCommand) == typeof(ClickCommand) &&
            model is ClickCommandModel clickCommandModel)
        {
            new WebDriverClickCommandHandler(_driver).Handle(clickCommandModel);
            return;
        }

        throw new NotSupportedException(
            $"Command '{typeof(TCommand).Name}' is not supported.");
    }
}


internal sealed class WebDriverClickCommandHandler
{
    private readonly WebDriverWait _wait;

    public WebDriverClickCommandHandler(IWebDriver driver)
    {

        ArgumentNullException.ThrowIfNull(driver);

        _wait = new WebDriverWait(
            driver,
            TimeSpan.FromSeconds(15));
    }

    public void Handle(ClickCommandModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        By by = model.Type switch
        {
            SelectorType.Css => By.CssSelector(model.Selector),
            SelectorType.XPath => By.XPath(model.Selector),
            _ => throw new NotSupportedException(
                $"Selector type '{model.Type}' is not supported.")
        };

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

                element.Click();

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
