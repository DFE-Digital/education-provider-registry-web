using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Web.DataPipeline.Application.Models;
using DfE.EducationProviderRegistry.Web.DataPipeline.Application.Services;

namespace DfE.EducationProviderRegistry.Web.DataPipeline.Application.UseCases;

/// <summary>
/// Orchestrates the ETL workflow for establishment data by coordinating the
/// extraction, loading, and outbox emission stages. This use case represents
/// the core application-level pipeline:
/// <list type="number">
/// <item><description>Extract canonical establishment records from the upstream source.</description></item>
/// <item><description>Persist the records into the read model store.</description></item>
/// <item><description>Emit outbox events for downstream indexing.</description></item>
/// </list>
/// </summary>
/// <remarks>
/// This use case contains no infrastructure concerns. All parsing, transformation,
/// persistence, and event writing are delegated to injected services. Exceptions
/// are captured and returned as structured <see cref="UseCaseResponse{TModel}"/>
/// results.
/// </remarks>
public sealed class EstablishmentsDataPipelineUseCase
    : IUseCaseResponseOnly<UseCaseResponse<object>>
{
    private readonly IEstablishmentDataExtractor _extractor;
    private readonly IEstablishmentLoader _loader;
    private readonly IOutboxEventWriter _outbox;

    /// <summary>
    /// Initialises a new instance of the <see cref="EstablishmentsDataPipelineUseCase"/> class.
    /// </summary>
    /// <param name="extractor">
    /// Component responsible for retrieving canonical establishment data from the upstream source.
    /// </param>
    /// <param name="loader">
    /// Component responsible for persisting establishment data into the read model store.
    /// </param>
    /// <param name="outbox">
    /// Component responsible for emitting outbox events for downstream indexing.
    /// </param>
    public EstablishmentsDataPipelineUseCase(
        IEstablishmentDataExtractor extractor,
        IEstablishmentLoader loader,
        IOutboxEventWriter outbox)
    {
        _extractor = extractor;
        _loader = loader;
        _outbox = outbox;
    }

    /// <summary>
    /// Executes the ETL workflow and returns a structured response indicating
    /// whether the operation succeeded or failed.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token that may be used to cancel the ETL operation.
    /// </param>
    /// <returns>
    /// A <see cref="UseCaseResponse{TModel}"/> containing either:
    /// <list type="bullet">
    /// <item><description>A success result with the number of processed establishments.</description></item>
    /// <item><description>A failure result with a descriptive error message.</description></item>
    /// </list>
    /// </returns>
    public async Task<UseCaseResponse<object>> HandleRequestAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            IEnumerable<EstablishmentReadModel> establishments =
                await _extractor.ExtractAsync(cancellationToken);

            if (!establishments.Any())
            {
                return UseCaseResponse<object>.Failure(
                    "No establishment records were extracted from the upstream source.");
            }

            await _loader.EnsureReadyAsync(cancellationToken);

            int processedCount = 0;

            foreach (EstablishmentReadModel establishment in establishments)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await _loader.UpsertAsync(establishment, cancellationToken);

                await _outbox.WriteAsync(
                    eventType: "EstablishmentUpdated",
                    entityId: establishment.Urn,
                    payload: establishment,
                    cancellationToken: cancellationToken);

                processedCount++;
            }

            IEnumerable<int> validIds =
                establishments.Select(establishment => establishment.Urn);

            await _loader.CleanupMissingAsync(validIds, cancellationToken);

            return UseCaseResponse<object>.Success(new { Count = processedCount });
        }
        catch (OperationCanceledException)
        {
            return UseCaseResponse<object>.Failure("The ETL operation was cancelled.");
        }
        catch (Exception ex)
        {
            return UseCaseResponse<object>.Failure(
                $"Unexpected ETL failure: {ex.Message}");
        }
    }
}
