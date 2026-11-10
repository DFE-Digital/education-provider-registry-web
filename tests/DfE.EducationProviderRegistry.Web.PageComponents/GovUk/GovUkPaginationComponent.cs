using DfE.Document.Abstractions;

namespace DfE.EducationProviderRegistry.Web.PageComponents.GovUk;

public sealed class GovUkPaginationComponent
{
    private readonly IQueryHandler _query;

    public GovUkPaginationComponent(IQueryHandler query)
    {
        ArgumentNullException.ThrowIfNull(query);
        _query = query;
    }

    public PaginationReadModel? Pages()
    {
        if (!Displayed())
        {
            return null;
        }

        int current = CurrentPage();

        IReadOnlyCollection<int> pages =
            _query.Handle((context)
                => context
                    .FindAll(".govuk-pagination__item")
                    .Select(element => int.Parse(element.Text.Trim()))
                    .ToArray());


        // TODO make fetching previous and next page numbers more robust, e.g. by using the href attribute of the link elements, than the text - OR - model them as links

        return new PaginationReadModel
        {
            Pages = pages,
            Current = current,
            Previous = DisplaysPreviousLink() ? current - 1 : null,
            Next = DisplaysNextLink() ? current + 1 : null
        };
    }

    private bool Displayed()
    {
        return _query.Handle(context => context.Find(".govuk-pagination") is not null);
    }

    private bool DisplaysNextLink()
    {
        return _query.Handle(context => context.Find(".govuk-pagination__next") is not null);
    }

    private bool DisplaysPreviousLink()
    {
        return _query.Handle(context => context.Find(".govuk-pagination__prev") is not null);
    }

    private int CurrentPage()
    {
        string current = _query.Handle((context) =>
            context.Find(".govuk-pagination__item--current")?.Text.Trim() ??
                throw new ArgumentException("Could not locate current page"));

        return int.Parse(current);
    }
}


public sealed record class PaginationReadModel
{
    public IReadOnlyCollection<int> Pages { get; init; } = [];
    public required int Current { get; init; }
    public int? Previous { get; init; }
    public int? Next { get; init; }
}
