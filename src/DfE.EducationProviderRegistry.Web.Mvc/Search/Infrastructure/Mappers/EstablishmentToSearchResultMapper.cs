using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using EstablishmentType = DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment.EstablishmentType;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Mappers;

public sealed class EstablishmentToSearchResultMapper : IMapper<Establishment, EstablishmentSearchResult>
{
    private static readonly string[] Towns =
    {
        "Newcastle", "Manchester", "Birmingham", "Leeds", "Sheffield",
        "Liverpool", "Nottingham", "Bristol", "Sunderland", "Durham"
    };

    private static readonly string[] Counties =
    {
        "Tyne and Wear", "Greater Manchester", "West Midlands",
        "West Yorkshire", "South Yorkshire", "Merseyside",
        "Nottinghamshire", "Bristol", "County Durham"
    };

    private static readonly string[] Streets =
    {
        "High Street", "Station Road", "Church Lane", "North Avenue",
        "School Road", "Park View", "Oak Drive", "Maple Crescent"
    };

    private static readonly Random Random = new();

    public EstablishmentSearchResult Map(Establishment input)
    {
        ArgumentNullException.ThrowIfNull(input);

        UniqueReferenceNumber urn = new(input.Urn.ToString());
        Name name = new(input.Name);

        string street = $"{Random.Next(1, 200)} {Streets[Random.Next(Streets.Length)]}";
        string town = Towns[Random.Next(Towns.Length)];
        string county = Counties[Random.Next(Counties.Length)];
        string postcode = GenerateRandomPostcode();

        Address address = new(
            Street: street,
            Town: town,
            County: county,
            Postcode: postcode
        );

        EstablishmentType type = new(Random.Next(0, 2) == 0 ? "Academy" : "Community School");

        GroupDetail group = new(
            partOfName: $"Trust {Random.Next(100, 999)}",
            partOfCode: $"T{Random.Next(1000, 9999)}"
        );

        LocalAuthority localAuthority = new(
            localAuthorityCode: Random.Next(800, 899).ToString(),
            localAuthorityName: $"{town} Council"
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

    private static string GenerateRandomPostcode()
    {
        // Simple but realistic UK postcode generator
        string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        char a = letters[Random.Next(letters.Length)];
        char b = letters[Random.Next(letters.Length)];
        int outward = Random.Next(1, 20);
        int inward = Random.Next(0, 9);
        char c = letters[Random.Next(letters.Length)];
        char d = letters[Random.Next(letters.Length)];

        return $"{a}{b}{outward} {inward}{c}{d}";
    }
}
