using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Groups.TestDoubles;

public static class GroupReadModelResponseTestDoubles
{
    public static GroupReadModelResponse Failure()
    {
        return new()
        {
            Group = null
        };
    }

    public static GroupReadModelResponse Stub()
    {
        return new GroupReadModelResponse()
        {
            Group = GroupReadModelTestDoubles.Stub()
        };
    }
}
