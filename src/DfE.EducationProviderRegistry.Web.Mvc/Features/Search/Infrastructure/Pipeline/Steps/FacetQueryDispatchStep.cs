using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Providers;
using System.Collections.ObjectModel;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Pipeline.Steps;

/// <summary>
/// Dispatches asynchronous facet queries for each requested facet name using the configured
/// <see cref="IFacetProvider{T}"/>. The resulting tasks are stored in the
/// <see cref="SearchPipelineContext"/> for later processing by <c>FacetQueryBuilderStep</c>.
/// </summary>
/// <remarks>
/// This step does not wait for facet tasks to complete. It simply schedules them and stores the
/// resulting task objects. Completion, fault handling, and result extraction are performed by
/// subsequent pipeline steps.
/// </remarks>
internal sealed class FacetQueryDispatchStep : ISearchPipelineStep
{
    private readonly IFacetProvider _facetProvider;

    /// <summary>
    /// Creates a new instance of <see cref="FacetQueryDispatchStep"/>.
    /// </summary>
    /// <param name="facetProvider">The facet provider used to execute facet queries.</param>
    public FacetQueryDispatchStep(
        IFacetProvider facetProvider)
    {
        _facetProvider = facetProvider;
    }

    /// <summary>
    /// Dispatches facet queries for each facet name found in the pipeline context. The resulting
    /// tasks are stored back into the context for later processing.
    /// </summary>
    /// <param name="context">The pipeline context containing establishment IDs and facet names.</param>
    /// <param name="cancellationToken">A token used to observe cancellation.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when required context entries are missing or invalid.
    /// </exception>
    public void Execute(SearchPipelineContext context, CancellationToken cancellationToken)
    {
        ReadOnlyCollection<string> ids =
            context.Get<ReadOnlyCollection<string>>()
            ?? throw new InvalidOperationException(
                "PipelineContext does not contain establishment IDs.");

        List<string> facetNames =
            context.Get<List<string>>()
            ?? throw new InvalidOperationException(
                "PipelineContext does not contain facet names.");

        if (facetNames.Count == 0)
        {
            context.Set(new List<(
                string FacetName, Task<IReadOnlyList<FacetResult>> Task)>());

            return;
        }

        List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)> tasks =
            new(facetNames.Count);

        foreach (string facetName in facetNames)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(facetName))
            {
                throw new InvalidOperationException(
                    "Facet name cannot be null or empty.");
            }

            Task<IReadOnlyList<FacetResult>> task =
                _facetProvider.GetFacetsAsync(ids, facetName, cancellationToken);

            tasks.Add((facetName, task));
        }

        context.Set(tasks);
    }
}
