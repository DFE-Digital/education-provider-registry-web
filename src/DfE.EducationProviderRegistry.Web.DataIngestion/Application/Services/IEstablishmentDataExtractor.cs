using DfE.EducationProviderRegistry.Web.DataPipeline.Application.Models;

namespace DfE.EducationProviderRegistry.Web.DataPipeline.Application.Services;

/// <summary>
/// Defines the extraction stage of the data pipeline. Implementations of this
/// interface are responsible for retrieving establishment data from an upstream
/// source (e.g., GIAS CSV, API, graph extract) and returning it in the canonical
/// <see cref="EstablishmentReadModel"/> format expected by the Application layer.
/// </summary>
/// <remarks>
/// This contract ensures that the Application layer remains completely agnostic
/// of upstream data formats. All parsing, validation, normalisation, and mapping
/// from raw source DTOs must be performed within the Infrastructure layer.
/// <para>
/// The extractor represents the first step of the ETL workflow:
/// <list type="number">
/// <item><description>Extract raw source data.</description></item>
/// <item><description>Transform it into <see cref="EstablishmentReadModel"/> instances.</description></item>
/// <item><description>Return the canonical models to the Application layer.</description></item>
/// </list>
/// </para>
/// </remarks>
public interface IEstablishmentDataExtractor
{
    /// <summary>
    /// Extracts establishment data from the configured upstream source and returns
    /// a collection of canonical <see cref="EstablishmentReadModel"/> instances.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token that may be used to cancel the extraction operation.
    /// </param>
    /// <returns>
    /// A collection of <see cref="EstablishmentReadModel"/> representing the fully
    /// normalised and transformed establishment data ready for loading into the
    /// persistence layer.
    /// </returns>
    Task<IEnumerable<EstablishmentReadModel>> ExtractAsync(
        CancellationToken cancellationToken);
}
