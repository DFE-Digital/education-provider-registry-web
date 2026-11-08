namespace DfE.Document.Abstractions;

public interface ICommandDispatcher
{
    void Dispatch<TCommand, TCommandModel>(TCommandModel model)
        where TCommand : ICommand<TCommandModel>
        where TCommandModel : notnull;
}
