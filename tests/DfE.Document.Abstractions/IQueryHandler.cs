namespace DfE.Document.Abstractions;

public interface IQueryHandler
{
    TResult Handle<TResult>(
        Func<ISearchContext, TResult> query);
}
