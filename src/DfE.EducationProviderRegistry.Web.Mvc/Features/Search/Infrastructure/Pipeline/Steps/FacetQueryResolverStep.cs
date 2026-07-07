using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Pipeline;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Pipeline.Steps;

/// <summary>
/// Ensures all facet query tasks dispatched earlier in the pipeline have completed.
/// This step waits for all facet tasks to finish and propagates any underlying exceptions.
/// </summary>
/// <remarks>
/// This step performs strict validation:
/// <list type="bullet">
/// <item><description>All facet tasks must be present in the pipeline context.</description></item>
/// <item><description>Cancellation is honoured before and during task waiting.</description></item>
/// <item><description>Faulted tasks are unwrapped and rethrown with their original exception.</description></item>
/// </list>
/// </remarks>
internal sealed class FacetQueryResolverStep : ISearchPipelineStep
{
    /// <summary>
    /// Waits for all facet query tasks to complete. Any faulted tasks are unwrapped and rethrown.
    /// </summary>
    /// <param name="context">The pipeline context containing facet query tasks.</param>
    /// <param name="cancellationToken">A token used to observe cancellation.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the pipeline context does not contain facet tasks.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Thrown when cancellation is requested.
    /// </exception>
    public void Execute(SearchPipelineContext context, CancellationToken cancellationToken)
    {
        List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)> tasks =
            context.Get<List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)>>()
            ?? throw new InvalidOperationException(
                "PipelineContext does not contain facet query tasks.");

        cancellationToken.ThrowIfCancellationRequested();

        Task[] taskArray = new Task[tasks.Count];
        for (int i = 0; i < tasks.Count; i++)
        {
            taskArray[i] = tasks[i].Task;
        }

        try
        {
            Task.WhenAll(taskArray).Wait(cancellationToken);
        }
        catch (AggregateException ex)
        {
            Exception inner = ex.InnerException ?? ex;
            throw new InvalidOperationException(
                "One or more facet tasks failed during resolution.", inner);
        }
    }
}
