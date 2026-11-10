using DfE.Document.Abstractions;
using DfE.EducationProviderRegistry.Web.PageComponents.Commands;

namespace DfE.EducationProviderRegistry.Web.PageComponents.Pages;

public sealed class SearchPanelComponent
{
    private readonly IQueryHandler _queryHandler;
    private readonly ICommandHandler _commandDispatcher;

    private const string IdentityInput = "#SearchKeywords";
    private const string LocationInput = "#Address";
    private const string SortDropdown = "#sort";
    private const string SubmitSearch = ".dfe-search-panel__actions [type=submit]";

    public SearchPanelComponent(
        IQueryHandler queryHandler,
        ICommandHandler commandDispatcher)
    {
        ArgumentNullException.ThrowIfNull(queryHandler);
        ArgumentNullException.ThrowIfNull(commandDispatcher);

        _queryHandler = queryHandler;
        _commandDispatcher = commandDispatcher;
    }

    public void Search((string identityTerm, string locationTerm) terms)
    {
        EnterTerm(terms.identityTerm, IdentityInput);
        EnterTerm(terms.locationTerm, LocationInput);
        Submit();
    }

    private void EnterTerm(string term, string selector)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return;
        }

        _commandDispatcher.Handle<SendKeysCommand, SendKeysCommandModel>(new()
        {
            Selector = selector,
            Text = term
        });
    }

    private void Submit()
    {
        _commandDispatcher.Handle<ClickCommand, ClickCommandModel>(new()
        {
            Selector = SubmitSearch
        });
    }

    public void SortBy(string _, SortDirection descending)
    {
        string sortValue = descending switch
        {
            SortDirection.Descending => "za",
            _ => "az"
        };

        _commandDispatcher.Handle<SelectValueInDropdownCommand, SelectValueInDropdownCommandModel>(new()
        {
            Selector = SortDropdown,
            Value = sortValue
        });
    }

    public enum SortDirection
    {
        Ascending,
        Descending
    }
}
