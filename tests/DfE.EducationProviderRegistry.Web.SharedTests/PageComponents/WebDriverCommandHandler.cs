using DfE.Document.Abstractions;
using DfE.EducationProviderRegistry.Web.PageComponents.Commands;
using OpenQA.Selenium;

namespace DfE.EducationProviderRegistry.Web.SharedTests.PageComponents;

// Temporary in dispatcher until lifecycle retrieval and composition are linked.
public sealed class WebDriverCommandHandler : ICommandHandler
{
    private readonly IWebDriver _driver;

    public WebDriverCommandHandler(IWebDriver driver)
    {
        ArgumentNullException.ThrowIfNull(driver);
        _driver = driver;
    }

    public void Handle<TCommand, TCommandModel>(TCommandModel model)
        where TCommand : ICommand<TCommandModel>
        where TCommandModel : notnull
    {
        if (typeof(TCommand) == typeof(ClickCommand) &&
            model is ClickCommandModel clickCommandModel)
        {
            new ClickCommandHandler(_driver, new ClickCommandModelValidator())
                .Handle(clickCommandModel);
            return;
        }

        if (typeof(TCommand) == typeof(SendKeysCommand) &&
            model is SendKeysCommandModel sendKeysCommandModel)
        {
            new SendKeysCommandHandler(_driver, new SendKeysCommandModelValidator())
                .Handle(sendKeysCommandModel);
            return;
        }

        if (typeof(TCommand) == typeof(SelectDropdownValueCommand) &&
            model is SelectDropdownValueCommandModel selectValueCommandModel)
        {
            new SelectDropdownValueCommandHandler(_driver, new SelectDropdownValueCommandModelValidator())
                .Handle(selectValueCommandModel);

            return;
        }

        throw new NotSupportedException($"Command '{typeof(TCommand).Name}' is not supported.");
    }
}





