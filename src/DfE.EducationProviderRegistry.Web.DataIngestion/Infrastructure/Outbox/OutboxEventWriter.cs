using DfE.EducationProviderRegistry.Web.DataPipeline.Application.Services;
using System.Text.Json;

namespace DfE.EducationProviderRegistry.Web.DataPipeline.Infrastructure.Outbox;

/// <summary>
/// Writes durable outbox messages into a PostgreSQL-backed outbox table.
/// These messages are later processed by downstream workers such as the
/// RedisSearch indexer.
/// </summary>
/// <remarks>
/// This implementation caches <see cref="JsonSerializerOptions"/> to avoid
/// repeated allocations and ensures that event messages are persisted in a
/// durable and idempotent manner.
/// </remarks>
public sealed class OutboxEventWriter : IOutboxEventWriter
{
    private readonly OutboxDbContext _dbContext;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Creates a new instance of the <see cref="OutboxEventWriter"/> class.
    /// </summary>
    /// <param name="dbContext">
    /// The EF Core database context used to persist outbox messages.
    /// </param>
    public OutboxEventWriter(OutboxDbContext dbContext)
    {
        _dbContext = dbContext;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    /// <summary>
    /// Writes a durable outbox event containing the specified payload.
    /// </summary>
    /// <param name="eventType">
    /// A string identifying the type of event (for example, "EstablishmentUpdated").
    /// </param>
    /// <param name="entityId">
    /// The unique identifier of the entity associated with the event.
    /// </param>
    /// <param name="payload">
    /// The event payload to be serialised and stored.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that may be used to cancel the write operation.
    /// </param>
    public async Task WriteAsync(
        string eventType,
        int entityId,
        object payload,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(eventType)){
            throw new ArgumentException(
                "Event type cannot be null or empty.", nameof(eventType));
        }

        string jsonPayload = JsonSerializer.Serialize(payload, _jsonOptions);

        OutboxMessage message = new()
        {
            Id = Guid.NewGuid(),
            EventType = eventType,
            EntityId = entityId,
            Payload = jsonPayload,
            CreatedUtc = DateTime.UtcNow,
            ProcessedUtc = null
        };

        _dbContext.OutboxMessages.Add(message);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
