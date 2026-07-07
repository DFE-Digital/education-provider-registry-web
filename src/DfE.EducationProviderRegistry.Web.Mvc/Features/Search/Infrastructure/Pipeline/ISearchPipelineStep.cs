namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Pipeline;

public interface ISearchPipelineStep
{
    void Execute(SearchPipelineContext context, CancellationToken cancellationToken);
}
