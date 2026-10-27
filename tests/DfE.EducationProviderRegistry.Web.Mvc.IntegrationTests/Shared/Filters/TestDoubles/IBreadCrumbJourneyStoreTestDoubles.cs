using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Store;
using Moq;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Shared.Filters.TestDoubles;

internal static class IBreadCrumbJourneyStoreTestDoubles
{
    public static Mock<IBreadcrumbJourneyStore> Default() => new();
}
