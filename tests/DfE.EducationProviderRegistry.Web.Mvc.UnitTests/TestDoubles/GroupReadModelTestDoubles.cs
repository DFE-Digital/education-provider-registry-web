using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.Mappers;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.TestDoubles;

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
            Type = "Multi-academy trust",
            Academies = Array.Empty<Academy>(),
            Members = Array.Empty<MemberReadModel>(),
            Trustees = Array.Empty<TrusteeReadModel>()
        };
    }
}