using Bogus;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Response;

namespace DfE.EducationProviderRegistry.Web.SharedTests.TestDoubles.Groups;

public static class EstablishmentReadModelTestDoubles
{
    public static IReadOnlyList<EstablishmentReadModel> Create(int count = 1)
    {
        Faker faker = new();
        List<EstablishmentReadModel> establishments = new(count);

        for (int i = 0; i < count; i++)
        {
            establishments.Add(
                new EstablishmentReadModel
                {
                    Name = faker.Company.CompanyName(),
                    Urn = faker.Random.Number(100000, 999999).ToString()
                });
        }

        return establishments;
    }
}
