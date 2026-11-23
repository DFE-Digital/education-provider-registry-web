namespace DfE.Document.Abstractions;

public interface ICommand<in TCommandModel> where TCommandModel : notnull
{
}
