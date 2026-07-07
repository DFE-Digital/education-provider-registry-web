namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Pipeline;

/// <summary>
/// Represents a single executable step within the search pipeline.
/// Pipeline steps operate on a shared <see cref="SearchPipelineContext"/> and
/// may enrich it, transform it, or dispatch additional work required to
/// complete a search operation.
/// </summary>
/// <remarks>
/// Each step is executed in sequence by the search orchestrator. Steps must be
/// deterministic, side‑effect free (except for updating the pipeline context),
/// and EF‑safe when performing database operations. Implementations should not
/// block, perform long‑running work synchronously, or dispose resources owned
/// by the pipeline.
/// </remarks>
public interface ISearchPipelineStep
{
    /// <summary>
    /// Executes the pipeline step using the provided <paramref name="context"/>.
    /// </summary>
    /// <param name="context">
    /// The shared <see cref="SearchPipelineContext"/> containing all state
    /// accumulated so far in the pipeline. Implementations may read from or
    /// write to this context to contribute to the overall search result.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to observe cancellation requests. Implementations should
    /// honour this token when performing asynchronous or database‑bound work.
    /// </param>
    void Execute(SearchPipelineContext context, CancellationToken cancellationToken);
}