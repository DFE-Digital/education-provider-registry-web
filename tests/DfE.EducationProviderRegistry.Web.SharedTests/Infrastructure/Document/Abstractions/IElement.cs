namespace DfE.EducationProviderRegistry.Web.SharedTests.Infrastructure.Document.Abstractions;

public interface IElement : ISearchContext
{
    string Text { get; }
    string? Attribute(string name);
}
