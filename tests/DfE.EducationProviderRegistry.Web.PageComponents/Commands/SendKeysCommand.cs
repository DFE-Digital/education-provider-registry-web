using DfE.Document.Abstractions;

namespace DfE.EducationProviderRegistry.Web.PageComponents.Commands;

public sealed class SendKeysCommand : ICommand<SendKeysCommandModel>
{
}

public sealed record class SendKeysCommandModel
{
    public required string Text { get; set; }
    public required string Selector { get; set; }
}
