using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers.SearchOrchestrators.EFMetadataResolver;

/// <summary>
/// Defines the contract for resolving EF Core metadata associated with a given
/// projection or entity type. Implementations provide access to the underlying
/// <see cref="IEntityType"/> model, table mapping, schema, and primary key metadata
/// required by search orchestrators.
/// </summary>
/// <typeparam name="TProjection">
/// The entity or projection type for which metadata should be resolved.
/// Must be a reference type.
/// </typeparam>
/// <remarks>
/// This abstraction allows search orchestrators to remain agnostic of how metadata
/// is retrieved or cached. Implementations may perform reflection, consult the
/// <see cref="DbContext"/> model, or use cached metadata snapshots.
/// </remarks>
public interface IEntityMetadataResolver<TProjection>
    where TProjection : class
{
    /// <summary>
    /// Resolves the EF Core metadata for the specified projection type using the
    /// provided <see cref="DbContext"/> instance.
    /// </summary>
    /// <param name="db">
    /// The EF Core <see cref="DbContext"/> whose model is used to resolve metadata.
    /// </param>
    /// <returns>
    /// A fully populated <see cref="EntityMetadata"/> instance describing the entity's
    /// schema, table name, primary key property, and associated column mappings.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="db"/> is null.
    /// </exception>
    EntityMetadata Resolve(DbContext db);
}
