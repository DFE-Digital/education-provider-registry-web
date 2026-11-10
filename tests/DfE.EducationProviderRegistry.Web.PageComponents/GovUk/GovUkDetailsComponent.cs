using DfE.Document.Abstractions;
using DfE.EducationProviderRegistry.Web.PageComponents.Commands;

namespace DfE.EducationProviderRegistry.Web.PageComponents.GovUk;

public sealed class GovUkDetailsComponent
{
    private readonly ICommandDispatcher _commandDispatcher;

    private const string SummaryTextSelector = ".govuk-details__summary-text";

    public GovUkDetailsComponent(ICommandDispatcher commandDispatcher)
    {
        ArgumentNullException.ThrowIfNull(commandDispatcher);

        _commandDispatcher = commandDispatcher;
    }

    public void Expand(string label)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(label);

        _commandDispatcher.Dispatch<ClickCommand, ClickCommandModel>(new()
        {
            Selector = SummaryTextSelector,
            WithText = label
        });
    }
}
