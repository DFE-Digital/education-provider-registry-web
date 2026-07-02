using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Providers.Projections;
using EstablishmentType = DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment.EstablishmentType;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Mappers;

public sealed class EstablishmentToSearchResultMapper : IMapper<Establishment, EstablishmentSearchResult>
{
    public EstablishmentSearchResult Map(Establishment input)
    {
        ArgumentNullException.ThrowIfNull(input);

        UniqueReferenceNumber urn = new(input.Urn);
        Name name = new(input.Name);

        Site site = input.Site.FirstOrDefault();

        Address address = new(
            Street: site.AddressLine1,
            Town: site.Town,
            County: site.County,
            Postcode: site.Postcode
        );

        EstablishmentType type = new(input.EstablishmentType.Name);

        GroupDetail group = new(
            partOfName:"TEST", //input.EstablishmentGroupMembership..GroupName,
            partOfCode: "TEST"//input.GroupCode
        );

        LocalAuthority localAuthority = new(
            localAuthorityCode: input.EstablishmentAuthority.FirstOrDefault().AuthorityCode,
            localAuthorityName: input.EstablishmentAuthority.FirstOrDefault().AuthorityName
        );

        return new EstablishmentSearchResult(
            urn,
            name,
            address,
            type,
            group,
            localAuthority
        );
    }
}
