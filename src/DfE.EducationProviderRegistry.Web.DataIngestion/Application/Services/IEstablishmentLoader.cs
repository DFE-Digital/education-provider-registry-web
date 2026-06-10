using DfE.EducationProviderRegistry.Web.DataPipeline.Application.Models;

namespace DfE.EducationProviderRegistry.Web.DataPipeline.Application.Services;

/// <summary>
/// Defines the loading stage of the data pipeline. Implementations of this
/// interface are responsible for persisting canonical establishment data into
/// the system's read model store (e.g., SQL database).
/// </summary>
/// <remarks>
/// The loader represents the downstream boundary of the ETL workflow. It receives
/// fully normalised <see cref="EstablishmentReadModel"/> instances from the
/// Application layer and ensures they are written to the persistence layer in a
/// consistent and idempotent manner.
/// <para>
/// Typical responsibilities include:
/// <list type="bullet">
/// <item><description>Upserting establishment records.</description></item>
/// <item><description>Removing records that no longer exist upstream.</description></item>
/// <item><description>Ensuring the persistence layer is ready before loading begins.</description></item>
/// </list>
/// </para>
/// </remarks>
public interface IEstablishmentLoader
{
    /// <summary>
    /// Ensures that the persistence layer is ready to receive establishment data.
    /// This may include creating tables, applying migrations, or performing
    /// integrity checks.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token that may be used to cancel the initialisation operation.
    /// </param>
    Task EnsureReadyAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Inserts or updates the specified <see cref="EstablishmentReadModel"/> in the
    /// persistence layer. Implementations must guarantee idempotency so that
    /// repeated ETL runs do not produce inconsistent state.
    /// </summary>
    /// <param name="establishment">
    /// The canonical establishment record to be persisted.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that may be used to cancel the upsert operation.
    /// </param>
    Task UpsertAsync(
        EstablishmentReadModel establishment,
        CancellationToken cancellationToken);

    /// <summary>
    /// Removes any establishments from the persistence layer whose identifiers
    /// are not present in the supplied collection. This ensures that the read
    /// model store remains consistent with the upstream source after an ETL run.
    /// </summary>
    /// <param name="validEstablishmentIds">
    /// A collection of establishment identifiers that were successfully extracted
    /// during the current ETL run.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that may be used to cancel the cleanup operation.
    /// </param>
    Task CleanupMissingAsync(
        IEnumerable<int> validEstablishmentIds,
        CancellationToken cancellationToken);
}

