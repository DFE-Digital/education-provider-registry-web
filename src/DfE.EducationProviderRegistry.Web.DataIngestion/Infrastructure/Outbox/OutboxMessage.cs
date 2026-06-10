namespace DfE.EducationProviderRegistry.Web.DataPipeline.Infrastructure.Outbox;

/// <summary>
/// Represents a durable outbox message stored in the database. Outbox messages
/// are written during the ETL process and later consumed by downstream workers
/// such as the RedisSearch indexer.
/// </summary>
/// <remarks>
/// This entity is persisted in the <c>outbox_messages</c> table and follows the
/// Outbox Pattern to ensure reliable, idempotent event propagation.
/// </remarks>
public sealed class OutboxMessage
{
    /// <summary>
    /// Gets or sets the unique identifier for the outbox message.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the type of event represented by this message
    /// (for example, <c>EstablishmentUpdated</c>).
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the entity associated with this event.
    /// </summary>
    /// <remarks>
    /// This value links the outbox message to the entity that triggered it.
    /// </remarks>
    public int EntityId { get; set; }

    /// <summary>
    /// Gets or sets the JSON‑serialised payload containing the event data.
    /// </summary>
    public string Payload { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the UTC timestamp indicating when the message was created.
    /// </summary>
    public DateTime CreatedUtc { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp indicating when the message was processed.
    /// A <c>null</c> value indicates that the message has not yet been processed.
    /// </summary>
    public DateTime? ProcessedUtc { get; set; }
}
