using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Web.DataPipeline.Infrastructure.Outbox;

/// <summary>
/// Represents the Entity Framework Core database context responsible for
/// managing the outbox message store. This context provides access to the
/// <see cref="OutboxMessage"/> entity and applies the required schema
/// configuration for the outbox table.
/// </summary>
/// <remarks>
/// The outbox table is used to persist durable event messages as part of the
/// Outbox Pattern. These messages are later processed by downstream workers
/// such as the RedisSearch indexer.
/// </remarks>
public sealed class OutboxDbContext : DbContext
{
    /// <summary>
    /// Initialises a new instance of the <see cref="OutboxDbContext"/> class
    /// using the specified database context options.
    /// </summary>
    /// <param name="options">
    /// The configuration options used to initialise the database context.
    /// </param>
    public OutboxDbContext(DbContextOptions<OutboxDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets the set of <see cref="OutboxMessage"/> entities representing the
    /// durable outbox messages stored in the database.
    /// </summary>
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    /// <summary>
    /// Configures the entity model for the outbox table, including table name,
    /// primary key, and required properties.
    /// </summary>
    /// <param name="modelBuilder">
    /// The model builder used to configure entity mappings.
    /// </param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("outbox_messages");

            entity.HasKey(outboxMessage => outboxMessage.Id);

            entity.Property(outboxMessage => outboxMessage.EventType)
                .IsRequired();

            entity.Property(outboxMessage => outboxMessage.Payload)
                .IsRequired();

            entity.Property(outboxMessage => outboxMessage.CreatedUtc)
                .IsRequired();
        });
    }
}
