using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.Mappers;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Groups;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;

namespace DfE.EducationProviderRegistry.Web.Mvc.Extensions;

public static class GroupsServiceCollectionExtensions
{
    public static IServiceCollection AddGroups(this IServiceCollection services)
    {
        services.AddSingleton<
            IMapper<GroupReadModel, GroupDetailsPageViewModel>,
            GroupDetailsPageViewModelMapper>();
        
        services.AddSingleton<
            IMapper<GroupBasicDetailsDto, GovUkTable>,
            GroupDetailsBasicDetailsTableMapper>();
        
        services.AddSingleton<
            IMapper<IEnumerable<Academy>, GovUkTable>,
            GroupDetailsAcademiesTableMapper>();
        
        services.AddSingleton<
            IMapper<IEnumerable<TrusteeReadModel>, GovUkTable>,
            GroupDetailsTrusteesTableMapper>();
        
        services.AddSingleton<
            IMapper<IEnumerable<MemberReadModel>, GovUkTable>,
            GroupDetailsMembersTableMapper>();

        services.AddSingleton<IGroupsRepository, StubGroupsRepository>();
        services.AddGroupsUseCaseDependencies();
        services.AddGroupsInfrastructureDependencies();
        return services;
    }
}

// Temporary Group Repository
public sealed class StubGroupsRepository : IGroupsRepository
{
    public Task<Group?> GetGroupByGroupIdAsync(GroupId groupId, CancellationToken cancellationToken = default)
    {
        Group groupDummy = new(
            groupId,
            new GroupUID(1),
            new CompaniesHouseId("09876543"),
            [
                new(
                    new AcademyId(new UniqueReferenceNumber("100001")),
                    new AcademyName("St Mary's Primary")
                ),
                new(
                    new AcademyId(new UniqueReferenceNumber("100002")),
                    new AcademyName("St John's Academy")
                )
            ],
            [
                new(
                    new GovernanceIdentifier("1000001"),
                    new Name("Sarah Green"),
                    new DateTime(2019, 02, 10)
                ),
                new(
                    new GovernanceIdentifier("1000002"),
                    new Name("John White"),
                    new DateTime(2020, 06, 22)
                )
            ],
            [
                new(
                    new GovernanceIdentifier("1000003"),
                    new Name("Alice Brown"),
                    new DateTime(2020, 01, 21),
                    new("Chair")
                ),
                new(
                    new GovernanceIdentifier("1000004"),
                    new Name("David Smith"),
                    new DateTime(2021, 03, 15),
                    new("CFO")
                )
            ]
        );

        return Task.FromResult(groupDummy);
    }
}