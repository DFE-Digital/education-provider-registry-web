using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;

namespace DfE.EducationProviderRegistry.Web.Mvc.ViewModels.Pages;

public class EstablishmentDetailsPageViewModel
{
    // Basic details table
    public EstablishmentBasicDetailsViewModel BasicDetails { get; set; }
    public List<EstablishmentGovernorViewModel> Governors { get; set; }
    public List<EstablishmentHistoryViewModel> History { get; set; }

    public GovUkTable BasicDetailsTable { get; set; }
    public GovUkTable GovernorsTable { get; set; }
    public GovUkTable HistoryTable { get; set; }
}

public class EstablishmentBasicDetailsViewModel
{
    public string Name { get; set; }
    public string Urn { get; set; }
    public string Ukprn { get; set; }
    public string DfeNumber { get; set; }
    public string Status { get; set; }
    public string Address { get; set; }
    public string LocalAuthority { get; set; }
    public string LocalAuthorityCode { get; set; }
    public string LocalAuthorityUrl => $"/la/{LocalAuthorityCode}";
    public string PartOfName { get; set; }
    public string PartOfCode { get; set; }
    public string PartOfUrl => $"/establishment/{PartOfCode}";
    public bool HasPartOf =>
        !string.IsNullOrWhiteSpace(PartOfName) &&
        !string.IsNullOrWhiteSpace(PartOfCode);
    public string Type { get; set; }
    public string PhaseOfEducation { get; set; }
    public string AgeRange { get; set; }
    public string Gender { get; set; }
    public string Religiouscharacter { get; set; }
    public string OfstedLastReported { get; set; }
    public string OfstedLastReportedUrl { get; set; }
}

public class EstablishmentGovernorViewModel
{
    public string Name { get; set; }
    public string GovernorId { get; set; }
    public string StartDate { get; set; }
}

public class EstablishmentHistoryViewModel
{
    public string HistoricName { get; set; }
    public string HistoricUrn { get; set; }
    public string HistoricUrl => $"/establishment/{HistoricUrn}";
    public string HistoricStatus { get; set; }
}