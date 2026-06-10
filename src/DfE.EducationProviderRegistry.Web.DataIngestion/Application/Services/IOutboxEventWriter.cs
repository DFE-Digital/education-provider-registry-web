namespace DfE.EducationProviderRegistry.Web.DataPipeline.Application.Services;

/// <summary>
/// Defines the contract for writing outbox events as part of the ETL workflow.
/// Implementations of this interface persist event messages that will later be
/// processed by an indexing or integration worker (e.g., RedisSearch indexer).
/// </summary>
/// <remarks>
/// This interface represents the boundary between the Application layer and the
/// infrastructure responsible for durable event storage. It ensures that the
/// Application layer remains agnostic of the underlying persistence mechanism
/// (e.g., SQL outbox table, message queue, event log).
/// <para>
/// Typical responsibilities of an outbox writer include:
/// <list type="bullet">
/// <item><description>Persisting event metadata and payloads in a durable store.</description></item>
/// <item><description>Ensuring idempotency for repeated ETL runs.</description></item>
/// <item><description>Serialising payloads in a format suitable for downstream consumers.</description></item>
/// </list>
/// </para>
/// </remarks>
public interface IOutboxEventWriter
{
    /// <summary>
    /// Writes an outbox event representing a change to an establishment record.
    /// The event is stored durably for later processing by downstream workers.
    /// </summary>
    /// <param name="eventType">
    /// A string identifying the type of event (e.g., "EstablishmentUpdated").
    /// </param>
    /// <param name="entityId">
    /// The unique identifier of the entity associated with the event.
    /// </param>
    /// <param name="payload">
    /// The event payload containing the canonical establishment data to be
    /// consumed by downstream processors.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that may be used to cancel the write operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous write operation.
    /// </returns>
    Task WriteAsync(
        string eventType,
        int entityId,
        object payload,
        CancellationToken cancellationToken);
}

