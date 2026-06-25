namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Pipeline;

public interface ISearchPipelineStep
{
    void Execute(SearchPipelineContext context, CancellationToken cancellationToken);
}
