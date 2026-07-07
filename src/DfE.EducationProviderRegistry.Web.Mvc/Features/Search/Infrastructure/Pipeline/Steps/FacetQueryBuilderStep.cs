using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Pipeline.Steps;

/// <summary>
/// Builds <see cref="SearchFacet"/> instances from completed facet query tasks stored in the
/// <see cref="SearchPipelineContext"/>.
/// </summary>
/// <remarks>
/// This step expects the pipeline context to contain a collection of facet query tasks produced by
/// <c>FacetQueryDispatchStep</c>. Each item consists of:
/// <list type="bullet">
/// <item><description>The facet name.</description></item>
/// <item><description>A completed <see cref="Task{TResult}"/> returning facet results.</description></item>
/// </list>
/// <para>
/// The step performs strict validation:
/// </para>
/// <list type="bullet">
/// <item><description>Tasks must be completed before this step executes.</description></item>
/// <item><description>Faulted tasks are rethrown with their original exception.</description></item>
/// <item><description>Null facet results are not permitted.</description></item>
/// </list>
/// <para>
/// The resulting <see cref="SearchFacet"/> collection is stored back into the pipeline context.
/// </para>
/// </remarks>
internal sealed class FacetQueryBuilderStep : ISearchPipelineStep
{
    /// <summary>
    /// Converts completed facet query tasks into <see cref="SearchFacet"/> objects and stores them
    /// in the pipeline context.
    /// </summary>
    /// <param name="context">The pipeline context containing facet query tasks.</param>
    /// <param name="cancellationToken">A token used to observe cancellation.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a facet task is incomplete, faulted, or returns null results.
    /// </exception>
    public void Execute(SearchPipelineContext context, CancellationToken cancellationToken)
    {
        List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)> tasks =
            context.Get<List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)>>();

        List<SearchFacet> facets = new(tasks.Count);

        foreach ((string facetName, Task<IReadOnlyList<FacetResult>> task) in tasks)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!task.IsCompleted)
            {
                throw new InvalidOperationException(
                    $"Facet task for '{facetName}' was not completed before FacetQueryBuilderStep.");
            }

            if (task.IsFaulted)
            {
                Exception inner = task.Exception?.InnerException ?? task.Exception!;
                throw new InvalidOperationException(
                    $"Facet task for '{facetName}' failed.", inner);
            }

            IReadOnlyList<FacetResult> results = task.Result
                ?? throw new InvalidOperationException(
                    $"Facet provider returned null results for facet '{facetName}'.");

            facets.Add(new SearchFacet(facetName, [.. results]));
        }

        context.Set(facets);
    }
}
