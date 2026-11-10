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

        if (typeof(TCommand) == typeof(SendKeysCommand) &&
            model is SendKeysCommandModel sendKeysCommandModel)
        {
            new WebDriverSendKeysCommandHandler(_driver).Handle(sendKeysCommandModel);
            return;
        }

        if (typeof(TCommand) == typeof(SelectValueInDropdownCommand) &&
            model is SelectValueInDropdownCommandModel selectValueCommandModel)
        {
            new WebDriverSelectValueInDropdownCommandHandler(_driver)
                .Handle(selectValueCommandModel);

            return;
        }

        throw new NotSupportedException($"Command '{typeof(TCommand).Name}' is not supported.");
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

    private static By GetBy(string selector, SelectorType type)
    {
        return type switch
        {
            SelectorType.Css => By.CssSelector(selector),
            SelectorType.XPath => By.XPath(selector),
            _ => throw new NotSupportedException(
                $"Selector type '{type}' is not supported.")
        };
    }
}

internal sealed class WebDriverSendKeysCommandHandler
{
    private readonly WebDriverWait _wait;

    public WebDriverSendKeysCommandHandler(IWebDriver driver)
    {
        ArgumentNullException.ThrowIfNull(driver);

        _wait = new WebDriverWait(
            driver,
            TimeSpan.FromSeconds(15));
    }

    public void Handle(SendKeysCommandModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

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

internal sealed class WebDriverSelectValueInDropdownCommandHandler
{
    private readonly WebDriverWait _wait;

    public WebDriverSelectValueInDropdownCommandHandler(IWebDriver driver)
    {
        ArgumentNullException.ThrowIfNull(driver);

        _wait = new WebDriverWait(
            driver,
            TimeSpan.FromSeconds(15));
    }

    public void Handle(SelectValueInDropdownCommandModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

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
