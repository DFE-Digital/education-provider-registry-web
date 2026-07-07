using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Pipeline.Steps;

/// <summary>
/// Maps EF <see cref="Establishment"/> entities to <see cref="EstablishmentSearchResult"/> DTOs
/// in parallel while preserving ordering.
/// </summary>
/// <remarks>
/// This step performs strict validation and ensures:
/// <list type="bullet">
/// <item><description>The pipeline context contains an ordered list of establishments.</description></item>
/// <item><description>Cancellation is honoured inside the parallel loop.</description></item>
/// <item><description>Mapping exceptions are surfaced immediately.</description></item>
/// </list>
/// </remarks>
internal sealed class ParallelMappingStep : ISearchPipelineStep
{
    private readonly IMapper<Establishment, EstablishmentSearchResult> _mapper;

    /// <summary>
    /// Creates a new instance of <see cref="ParallelMappingStep"/>.
    /// </summary>
    /// <param name="mapper">The mapper used to convert establishments into search results.</param>
    public ParallelMappingStep(
        IMapper<Establishment, EstablishmentSearchResult> mapper)
    {
        _mapper = mapper;
    }

    /// <summary>
    /// Maps all establishments in the pipeline context to <see cref="EstablishmentSearchResult"/>
    /// instances in parallel while preserving the original ordering.
    /// </summary>
    /// <param name="context">The pipeline context containing ordered establishments.</param>
    /// <param name="cancellationToken">A token used to observe cancellation.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the pipeline context does not contain establishments.
    /// </exception>
    public void Execute(SearchPipelineContext context, CancellationToken cancellationToken)
    {
        IReadOnlyList<Establishment> ordered =
            context.Get<IReadOnlyList<Establishment>>()
            ?? throw new InvalidOperationException(
                "PipelineContext does not contain ordered establishments.");

        EstablishmentSearchResult[] results =
            new EstablishmentSearchResult[ordered.Count];

        ParallelOptions options = new()
        {
            CancellationToken = cancellationToken
        };

        Parallel.ForEach(
            Enumerable.Range(0, ordered.Count),
            options,
            index =>
            {
                options.CancellationToken.ThrowIfCancellationRequested();

                Establishment establishment = ordered[index];

                EstablishmentSearchResult mapped =
                    _mapper.Map(establishment);

                results[index] = mapped;
            });

        context.Set(results);
    }
}
