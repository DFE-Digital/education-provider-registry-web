namespace DfE.EducationProviderRegistry.Web.DataPipeline.Infrastructure.Extraction.Mapping;

/// <summary>
/// Represents a single row from the GIAS daily extract CSV.
/// </summary>
public sealed class GiasEstablishmentRow
{
    public int Urn { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string LocalAuthority { get; set; } = string.Empty;
    public string Postcode { get; set; } = string.Empty;
}
