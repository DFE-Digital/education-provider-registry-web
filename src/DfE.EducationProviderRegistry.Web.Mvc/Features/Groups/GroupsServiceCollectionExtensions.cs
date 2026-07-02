using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Groups;

public static class GroupsServiceCollectionExtensions
{
    public static IServiceCollection AddGroups(this IServiceCollection services)
    {
        services.AddSingleton<
            IMapper<GroupReadModel, GroupDetailsPageViewModel>,
            GroupDetailsPageViewModelMapper>();

        services.AddGroupsUseCaseDependencies();
        services.AddGroupsInfrastructureDependencies();
        return services;
    }
}