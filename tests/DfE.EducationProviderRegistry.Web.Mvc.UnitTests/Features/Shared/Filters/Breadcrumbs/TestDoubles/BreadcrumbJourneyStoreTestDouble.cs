using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Store;
using Moq;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Filters.Breadcrumbs.TestDoubles;

[ExcludeFromCodeCoverage]
public static class BreadcrumbJourneyStoreTestDouble
{
    public static Mock<IBreadcrumbJourneyStore> Mock()
    {
        return new Mock<IBreadcrumbJourneyStore>();
    }

    public static Mock<IBreadcrumbJourneyStore> MockFor(
        BreadcrumbJourney journey)
    {
        Mock<IBreadcrumbJourneyStore> mock = new();

        mock.Setup(store => store.Load())
            .Returns(journey);

        return mock;
    }
}
