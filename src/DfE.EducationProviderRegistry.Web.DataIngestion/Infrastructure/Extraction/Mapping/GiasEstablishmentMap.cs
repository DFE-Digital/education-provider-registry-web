using CsvHelper.Configuration;

namespace DfE.EducationProviderRegistry.Web.DataPipeline.Infrastructure.Extraction.Mapping;


/// <summary>
/// Strongly typed mapping for the GIAS daily extract CSV.
/// </summary>
public sealed class GiasEstablishmentMap : ClassMap<GiasEstablishmentRow>
{
    public GiasEstablishmentMap()
    {
        Map(establishmentRow => establishmentRow.Urn).Name("URN");
        Map(establishmentRow => establishmentRow.Name).Name("EstablishmentName");
    }
}
