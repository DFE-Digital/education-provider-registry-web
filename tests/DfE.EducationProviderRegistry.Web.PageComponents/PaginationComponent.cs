using DfE.Document.Abstractions;

namespace DfE.EducationProviderRegistry.Web.PageComponents;

public sealed class PaginationComponent
{
    private readonly ISearchContext _context;

    public PaginationComponent(ISearchContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public PaginationReadModel? Pages()
    {
        if (!Displayed())
        {
            return null;
        }

        int current = int.Parse(CurrentPage());

        IReadOnlyCollection<int> items = _context
            .FindAll(".govuk-pagination__item")
            .Select(element => int.Parse(element.Text.Trim()))
            .ToArray();

        // TODO make fetching previous and next page numbers more robust, e.g. by using the href attribute of the link elements, than the text - OR - model them as links

        return new PaginationReadModel
        {
            Items = items,
            Current = current,
            Previous = DisplaysPreviousLink() ? current - 1 : null,
            Next = DisplaysNextLink() ? current + 1 : null
        };
    }

    private bool Displayed()
    {
        return _context.Find(".govuk-pagination") is not null;
    }

    private bool DisplaysNextLink()
    {
        return _context.Find(".govuk-pagination__next") is not null;
    }

    private bool DisplaysPreviousLink()
    {
        return _context.Find(".govuk-pagination__prev") is not null;
    }

    private string CurrentPage()
    {
        return _context.Find(".govuk-pagination__item--current")?.Text.Trim() ??
            throw new ArgumentException("Could not locate current page");
    }
}


public sealed record class PaginationReadModel
{
    public IReadOnlyCollection<int> Items { get; init; } = [];
    public required int Current { get; init; }
    public int? Previous { get; init; }
    public int? Next { get; init; }
}
