using DfE.EducationProviderRegistry.Web.DataPipeline.Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DfE.EducationProviderRegistry.Web.DataPipeline.Infrastructure.Persistence;

/// <summary>
/// Represents the Entity Framework Core database context responsible for
/// persisting canonical establishment read models. This context is used by the
/// ETL loader to maintain the read model store in PostgreSQL.
/// </summary>
/// <remarks>
/// The <see cref="EstablishmentDbContext"/> defines the schema mapping for
/// <see cref="EstablishmentReadModel"/> and ensures that the read model store
/// remains consistent with the upstream data source.
/// </remarks>
public sealed class EstablishmentDbContext : DbContext
{
    /// <summary>
    /// Initialises a new instance of the <see cref="EstablishmentDbContext"/> class
    /// using the specified database context options.
    /// </summary>
    /// <param name="options">
    /// The configuration options used to initialise the database context.
    /// </param>
    public EstablishmentDbContext(DbContextOptions<EstablishmentDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets the set of <see cref="EstablishmentReadModel"/> entities representing
    /// the canonical establishment records stored in the database.
    /// </summary>
    public DbSet<EstablishmentReadModel> Establishments => Set<EstablishmentReadModel>();

    /// <summary>
    /// Configures the entity model for the establishment read model table,
    /// including table name, primary key, and required properties.
    /// </summary>
    /// <param name="modelBuilder">
    /// The model builder used to configure entity mappings.
    /// </param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        EntityTypeBuilder<EstablishmentReadModel> entity =
            modelBuilder.Entity<EstablishmentReadModel>();

        entity.ToTable("establishments");
        entity.HasKey(establlishment => establlishment.Urn);
        entity.Property(establlishment => establlishment.Urn).IsRequired();
        entity.Property(establlishment => establlishment.Urn).IsRequired();
        entity.Property(establlishment => establlishment.Name).IsRequired();
    }
}
