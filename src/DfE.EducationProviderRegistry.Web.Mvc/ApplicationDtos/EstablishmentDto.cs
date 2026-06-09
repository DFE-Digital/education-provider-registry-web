namespace DfE.EducationProviderRegistry.Web.Mvc.ApplicationDtos;

public class EstablishmentDto
{
    public EstablishmentBasicDetailsDto BasicDetails { get; set; }
    public List<EstablishmentGovernorDto> Governors { get; set; }
    public List<EstablishmentHistoryDto> History { get; set; }
}

public class EstablishmentBasicDetailsDto
{
    public string Name { get; set; }
    public string Urn { get; set; }
    public string Ukprn { get; set; }
    public string DfeNumber { get; set; }
    public string Status { get; set; }
    public string Address { get; set; }
    public string LocalAuthority { get; set; }
    public string LocalAuthorityCode { get; set; }
    public string PartOfName { get; set; }
    public string PartOfCode { get; set; }
    public string Type { get; set; }
    public string PhaseOfEducation { get; set; }
    public string AgeRange { get; set; }
    public string Gender { get; set; }
    public string ReligiousCharacter { get; set; }
    public string OfstedLastReported { get; set; }
    public string OfstedLastReportedUrl { get; set; }
}

public class EstablishmentGovernorDto
{
    public string Name { get; set; }
    public string GovernorId { get; set; }
    public string StartDate { get; set; }
}

public class EstablishmentHistoryDto
{
    public string Name { get; set; }
    public string Urn { get; set; }
    public string Status { get; set; }
}