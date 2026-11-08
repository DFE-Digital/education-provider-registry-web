using DfE.Document.Abstractions;
using DfE.EducationProviderRegistry.Web.PageComponents.Commands;

namespace DfE.EducationProviderRegistry.Web.PageComponents;

public sealed class GovUkDetailsComponent
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryHandler _query;

    public GovUkDetailsComponent(IQueryHandler query, ICommandDispatcher commandDispatcher)
    {
        ArgumentNullException.ThrowIfNull(commandDispatcher);
        ArgumentNullException.ThrowIfNull(query);
        _query = query;
        _commandDispatcher = commandDispatcher;
    }

    public void Expand(string label)
    {
        string textSelector =
            $"//*[contains(concat(' ', normalize-space(@class), ' '), ' govuk-details__summary-text ') " +
            $"and contains(normalize-space(.), '{label}')]";


        _commandDispatcher.Dispatch<ClickCommand, ClickCommandModel>(new()
        {
            Selector = textSelector,
            Type = SelectorType.XPath
        });
    }
}
