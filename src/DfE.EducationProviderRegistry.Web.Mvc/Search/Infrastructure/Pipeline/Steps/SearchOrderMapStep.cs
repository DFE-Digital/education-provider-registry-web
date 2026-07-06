using System.Collections.ObjectModel;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Pipeline.Steps;

/// <summary>
/// Builds a dictionary mapping URNs to their positional order in the search results.
/// </summary>
/// <remarks>
/// This step performs strict validation:
/// <list type="bullet">
/// <item><description>The pipeline context must contain a list of establishment URNs.</description></item>
/// <item><description>Each URN must be non-null and non-empty.</description></item>
/// </list>
/// The resulting map is used by <c>SearchOrderingStep</c> to restore the correct ordering.
/// </remarks>
internal sealed class SearchOrderMapStep : ISearchPipelineStep
{
    /// <summary>
    /// Creates a positional order map for establishments based on their URNs.
    /// </summary>
    /// <param name="context">The pipeline context containing URNs.</param>
    /// <param name="cancellationToken">A token used to observe cancellation.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the pipeline context does not contain URNs or when any URN is null/empty.
    /// </exception>
    public void Execute(SearchPipelineContext context, CancellationToken cancellationToken)
    {
        ReadOnlyCollection<string> ids =
            context.Get<ReadOnlyCollection<string>>()
            ?? throw new InvalidOperationException(
                "PipelineContext does not contain establishment URNs.");

        Dictionary<string, int> orderMap = new(ids.Count);

        for (int index = 0; index < ids.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string urn = ids[index];

            if (string.IsNullOrWhiteSpace(urn))
            {
                throw new InvalidOperationException(
                    $"Encountered null or empty URN at index {index}.");
            }

            orderMap[urn] = index;
        }

        context.Set(orderMap);
    }
}
