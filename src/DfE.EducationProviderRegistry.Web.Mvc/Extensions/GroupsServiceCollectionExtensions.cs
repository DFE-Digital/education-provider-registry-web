using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById;
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
            new GroupIdentity(new GroupId("TR3566"), new GroupUID(3566)),
            new GroupExternalIdentifiers(
                new Ukprn("10058857"), 
                new CompaniesHouseId("07662289")),
            new GroupComposition(
                academies: [
                    new(
                        new AcademyId(new UniqueReferenceNumber("138997")),
                        new AcademyName("Chadsfield Green Academy")
                    ),
                    new(
                        new AcademyId(new UniqueReferenceNumber("103151")),
                        new AcademyName("Copperston Primary School")
                    ),
                    new(
                        new AcademyId(new UniqueReferenceNumber("103151")),
                        new AcademyName("St Mary Primary School")
                    ),
                    new(
                        new AcademyId(new UniqueReferenceNumber("105788")),
                        new AcademyName("Tywell Croft Academy")
                    )
                ],
                members: [
                    new(
                        new GovernanceIdentifier("1875454"),
                        new Name("Sarah Green"),
                        new DateTime(2019, 02, 10)
                    ),
                    new(
                        new GovernanceIdentifier("1000002"),
                        new Name("John White"),
                        new DateTime(2020, 06, 22)
                    )
                ],
                trustees: [
                    new(
                        new GovernanceIdentifier("1875454"),
                        new Name("Silas Thorne"),
                        new DateTime(2025, 08, 27),
                        new("Chair")
                    ),
                    new(
                        new GovernanceIdentifier("1000004"),
                        new Name("Isla Montgomery"),
                        new DateTime(2025, 05, 21),
                        new("CFO")
                    ),
                    new(
                        new GovernanceIdentifier("1875457"),
                        new Name("Caleb West"),
                        new DateTime(2025, 08, 27),
                        new("Accounting Officer")
                    )
            ]
        ),
            new GroupCharacteristics(
                new Name("The Park Academies Trust"),
                new Address("Phillipson Street", "Aston", "Birmingham", "B6 4PA"),
                new GroupType("Multi-academy trust"),
                new GroupStatus(GroupOpenState.Open, new DateTime(2011, 7, 5))));

        return Task.FromResult(groupDummy);
    }
}