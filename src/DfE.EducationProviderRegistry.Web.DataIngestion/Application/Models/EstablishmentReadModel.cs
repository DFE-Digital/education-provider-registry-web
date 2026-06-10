namespace DfE.EducationProviderRegistry.Web.DataPipeline.Application.Models;

/// <summary>
/// Represents the canonical, denormalised establishment record produced during the ETL
/// transformation stage. This model is used exclusively within the Application layer
/// as a read model and does not contain any domain behaviour.
/// </summary>
/// <remarks>
/// This record is intentionally simple and immutable. It is designed for:
/// <list type="bullet">
/// <item><description>ETL transformation output</description></item>
/// <item><description>Persistence into the read model store</description></item>
/// <item><description>Emission as an outbox event payload</description></item>
/// <item><description>Indexing into RedisSearch</description></item>
/// </list>
public sealed record EstablishmentReadModel(
    int Urn,
    string Name
);