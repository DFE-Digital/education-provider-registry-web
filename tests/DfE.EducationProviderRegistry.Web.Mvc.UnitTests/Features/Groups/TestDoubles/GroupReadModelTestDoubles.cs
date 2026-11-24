using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Response;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Groups.TestDoubles;

internal static class GroupReadModelTestDoubles
{
    internal static GroupReadModel Stub(
        string? name = null,
        string? groupId = null)
    {
        return new GroupReadModel
        {
            Name = name ?? "Test Group",
            GroupId = groupId ?? "group-1",
            GroupUID = 123,
            UKPRN = "10000001",
            CompaniesHouseId = "CH123456",
            Address = "1 Test Street",
            Status = "Open",
            TypeName = "Multi-academy trust",
            TypeCode = "02",
            Members = Array.Empty<MemberReadModel>(),
            Trustees = Array.Empty<TrusteeReadModel>(),
            Establishments = Array.Empty<EstablishmentReadModel>()
        };
    }
}
