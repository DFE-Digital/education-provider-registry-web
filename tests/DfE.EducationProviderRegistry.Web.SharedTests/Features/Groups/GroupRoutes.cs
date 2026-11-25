namespace DfE.EducationProviderRegistry.Web.SharedTests.Features.Groups;

public static class GroupRoutes
{
    public static Uri Details(long uid) => new($"groups/{uid}", UriKind.Relative);
}
