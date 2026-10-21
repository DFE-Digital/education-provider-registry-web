namespace DfE.EducationProviderRegistry.Web.SharedTests.Infrastructure.Document.Abstractions;

public interface ISearchContext
{
    IElement? Find(string selector);
    IReadOnlyCollection<IElement> FindAll(string selector);
}
