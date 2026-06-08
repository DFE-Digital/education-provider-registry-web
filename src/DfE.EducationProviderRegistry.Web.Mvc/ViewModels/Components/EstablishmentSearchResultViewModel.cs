namespace DfE.EducationProviderRegistry.Web.Mvc.ViewModels.Components;

public class EstablishmentSearchResultViewModel
{
    public string Name { get; set; }
    public string EstablishmentUrl => $"/establishment/{Urn}";

    public string Urn { get; set; }
    public string Type { get; set; }
    public string Address { get; set; }

    public string LocalAuthorityName { get; set; }
    public string LocalAuthorityCode { get; set; }
    public string LocalAuthorityUrl => $"/la/{LocalAuthorityCode}";

    public string PartOfName { get; set; }
    public string PartOfCode { get; set; }
    public string PartOfUrl => $"/establishment/{PartOfCode}";
    public bool HasPartOf =>
        !string.IsNullOrWhiteSpace(PartOfName) &&
        !string.IsNullOrWhiteSpace(PartOfCode);
}
