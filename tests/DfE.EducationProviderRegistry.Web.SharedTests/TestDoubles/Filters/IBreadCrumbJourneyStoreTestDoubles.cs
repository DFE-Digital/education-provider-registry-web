using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Store;
using Moq;

namespace DfE.EducationProviderRegistry.Web.SharedTests.TestDoubles.Filters;

public static class BreadcrumbJourneyStoreTestDouble
{
    public static Mock<IBreadcrumbJourneyStore> Mock() => new();
    public static IBreadcrumbJourneyStore Stub()
    {
        return new BreadcrumbJourneyStoreStub(
            new BreadcrumbJourney());
    }

    public static IBreadcrumbJourneyStore Stub(BreadcrumbJourney journey)
    {
        return new BreadcrumbJourneyStoreStub(journey);
    }

    private sealed class BreadcrumbJourneyStoreStub : IBreadcrumbJourneyStore
    {
        private BreadcrumbJourney _journey;

        public BreadcrumbJourneyStoreStub(
            BreadcrumbJourney journey)
        {
            ArgumentNullException.ThrowIfNull(journey);

            _journey = journey;
        }

        public BreadcrumbJourney Load()
        {
            return _journey;
        }

        public void Save(BreadcrumbJourney journey)
        {
            ArgumentNullException.ThrowIfNull(journey);

            _journey = journey;
        }

        public void Clear()
        {
            _journey = new BreadcrumbJourney();
        }
    }
}
