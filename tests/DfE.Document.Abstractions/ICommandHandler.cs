namespace DfE.Document.Abstractions;

public interface ICommandHandler
{
    void Handle<TCommand, TCommandModel>(TCommandModel model)
        where TCommand : ICommand<TCommandModel>
        where TCommandModel : notnull;
}
