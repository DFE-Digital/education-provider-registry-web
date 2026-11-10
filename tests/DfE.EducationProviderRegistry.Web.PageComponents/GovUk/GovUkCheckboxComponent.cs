using DfE.Document.Abstractions;
using DfE.EducationProviderRegistry.Web.PageComponents.Commands;

namespace DfE.EducationProviderRegistry.Web.PageComponents.GovUk;

public sealed class GovUkCheckboxComponent
{
    private readonly IQueryHandler _queryHandler;
    private readonly ICommandDispatcher _commandDispatcher;

    private const string CheckboxSelector = ".govuk-checkboxes";
    private const string CheckboxLabelSelector = ".govuk-checkboxes .govuk-checkboxes__label";

    public GovUkCheckboxComponent(
        IQueryHandler queryHandler,
        ICommandDispatcher commandDispatcher)
    {
        ArgumentNullException.ThrowIfNull(queryHandler);
        ArgumentNullException.ThrowIfNull(commandDispatcher);

        _queryHandler = queryHandler;
        _commandDispatcher = commandDispatcher;
    }

    public IReadOnlyCollection<IElement> Find()
    {
        return _queryHandler.Handle((context) =>
            context.FindAll(CheckboxSelector));
    }

    public void Click(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        _commandDispatcher.Dispatch<
            ClickCommand,
            ClickCommandModel>(new()
            {
                Selector = CheckboxLabelSelector,
                WithText = text
            });
    }
}
