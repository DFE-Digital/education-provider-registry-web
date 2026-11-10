using DfE.Document.Abstractions;
using DfE.EducationProviderRegistry.Web.PageComponents.Commands;

namespace DfE.EducationProviderRegistry.Web.PageComponents.GovUk;

public sealed class GovUkDetailsComponent
{
    private readonly ICommandHandler _commandDispatcher;

    private const string SummaryTextSelector = ".govuk-details__summary-text";

    public GovUkDetailsComponent(ICommandHandler commandDispatcher)
    {
        ArgumentNullException.ThrowIfNull(commandDispatcher);

        _commandDispatcher = commandDispatcher;
    }

    public void Expand(string label)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(label);

        _commandDispatcher.Handle<ClickCommand, ClickCommandModel>(new()
        {
            Selector = SummaryTextSelector,
            WithText = label
        });
    }
}
