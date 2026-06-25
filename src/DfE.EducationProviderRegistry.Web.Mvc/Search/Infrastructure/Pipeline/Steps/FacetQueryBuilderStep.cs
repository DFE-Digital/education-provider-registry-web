using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Pipeline.Steps;

internal sealed class FacetQueryBuilderStep : ISearchPipelineStep
{
    public void Execute(SearchPipelineContext context, CancellationToken cancellationToken)
    {
        List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)> tasks =
            context.Get<List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)>>();

        List<SearchFacet> facets =
            [.. tasks.Select(task =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return new SearchFacet(task.FacetName, [.. task.Task.Result]);
                })];

        context.Set(facets);
    }
}