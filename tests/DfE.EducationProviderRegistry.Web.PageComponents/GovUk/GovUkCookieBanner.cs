using DfE.Document.Abstractions;
using DfE.EducationProviderRegistry.Web.PageComponents.Commands;

namespace DfE.EducationProviderRegistry.Web.PageComponents.GovUk;

public sealed class GovUkCookieBanner
{
    private readonly ICommandDispatcher _commandDispatcher;

    public GovUkCookieBanner(ICommandDispatcher commandDispatcher)
    {
        ArgumentNullException.ThrowIfNull(commandDispatcher);
        _commandDispatcher = commandDispatcher;
    }

    public void Accept()
    {
        _commandDispatcher.Dispatch<ClickCommand, ClickCommandModel>(new()
        {
            Selector = ".govuk-cookie-banner button[value='true']"
        });
    }

    public void Reject()
    {
        _commandDispatcher.Dispatch<ClickCommand, ClickCommandModel>(new()
        {
            Selector = ".govuk-cookie-banner button[value='false']"
        });
    }
}
