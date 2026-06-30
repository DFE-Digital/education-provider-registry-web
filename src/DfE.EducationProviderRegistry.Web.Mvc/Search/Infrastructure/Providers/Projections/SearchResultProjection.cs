namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Providers.Projections;

public sealed record SearchResultProjection(
    int Id,
    string Name,
    string SearchColumn,
    string AddressLine1,
    string AddressLine2,
    string Town,
    string County,
    string Postcode,
    string EstablishmentType,
    string GroupCode,
    string GroupName,
    string AuthorityCode,
    string AuthorityName
);