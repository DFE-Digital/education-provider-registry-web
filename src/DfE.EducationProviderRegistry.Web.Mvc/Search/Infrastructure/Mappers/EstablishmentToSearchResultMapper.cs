using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using EstablishmentType = DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment.EstablishmentType;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Mappers;

/// <summary>
/// Maps an <see cref="Establishment"/> entity from the database model into an
/// <see cref="EstablishmentSearchResult"/> projection used by the search layer.
/// </summary>
/// <remarks>
/// This mapper performs strict validation of required nested components.  
/// The following collections must contain at least one element:
/// <list type="bullet">
/// <item><description><c>Site</c></description></item>
/// <item><description><c>EstablishmentGroupMembership</c></description></item>
/// <item><description><c>EstablishmentAuthority</c></description></item>
/// </list>
/// If any required component is missing, an <see cref="InvalidOperationException"/> is thrown.
/// </remarks>
public sealed class EstablishmentToSearchResultMapper : IMapper<Establishment, EstablishmentSearchResult>
{
    /// <summary>
    /// Maps the provided <see cref="Establishment"/> instance into an <see cref="EstablishmentSearchResult"/>.
    /// </summary>
    /// <param name="input">The establishment entity to map.</param>
    /// <returns>A fully populated <see cref="EstablishmentSearchResult"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="input"/> is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when required nested components such as site, group membership, or authority are missing.
    /// </exception>
    public EstablishmentSearchResult Map(Establishment input)
    {
        ArgumentNullException.ThrowIfNull(input);

        UniqueReferenceNumber urn = new UniqueReferenceNumber(input.Urn);
        Name name = new Name(input.Name);

        Site site = input.Site.FirstOrDefault()
            ?? throw new InvalidOperationException(
                "Establishment.Site must contain at least one site.");

        Address address =
            new Address(
                Street: site.AddressLine1,
                Town: site.Town,
                County: site.County,
                Postcode: site.Postcode
            );

        EstablishmentType type = new EstablishmentType(input.EstablishmentType.Name);

        EstablishmentGroupMembership membership = input.EstablishmentGroupMembership.FirstOrDefault()
            ?? throw new InvalidOperationException(
                "Establishment.EstablishmentGroupMembership must contain at least one group membership.");

        GroupRecord groupRecord = membership.Group
            ?? throw new InvalidOperationException(
                "GroupRecord cannot be null.");

        GroupDetail group =
            new GroupDetail(
                partOfName: groupRecord.Name,
                partOfCode: groupRecord.Code
            );

        EstablishmentAuthority authority =
            input.EstablishmentAuthority.FirstOrDefault()
                ?? throw new InvalidOperationException(
                    "Establishment.EstablishmentAuthority must contain at least one authority.");

        LocalAuthority localAuthority = new LocalAuthority(
            localAuthorityCode: authority.AuthorityCode,
            localAuthorityName: authority.AuthorityName
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
