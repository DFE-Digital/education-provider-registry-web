namespace DfE.Document.Abstractions;

// TODO hide
public sealed class QueryHandler : IQueryHandler
{
    private readonly ISearchContext _context;

    public QueryHandler(ISearchContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;
    }

    public TResult Handle<TResult>(Func<ISearchContext, TResult> query)
    {
        ArgumentNullException.ThrowIfNull(query);
        return query(_context);
    }
}
