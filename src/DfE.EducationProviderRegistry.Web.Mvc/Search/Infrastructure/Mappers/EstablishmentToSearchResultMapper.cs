using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Providers.Projections;
using EstablishmentType = DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment.EstablishmentType;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Mappers;

public sealed class EstablishmentToSearchResultMapper : IMapper<SearchResultProjection, EstablishmentSearchResult>
{
    public EstablishmentSearchResult Map(SearchResultProjection input)
    {
        ArgumentNullException.ThrowIfNull(input);

        UniqueReferenceNumber urn = new(input.Id.ToString());
        Name name = new(input.Name);

        Address address = new(
            Street: input.AddressLine1,
            Town: input.Town,
            County: input.County,
            Postcode: input.Postcode
        );

        EstablishmentType type = new(input.EstablishmentType);

        GroupDetail group = new(
            partOfName: input.GroupName,
            partOfCode: input.GroupCode
        );

        LocalAuthority localAuthority = new(
            localAuthorityCode: input.AuthorityCode,
            localAuthorityName: input.AuthorityName
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
