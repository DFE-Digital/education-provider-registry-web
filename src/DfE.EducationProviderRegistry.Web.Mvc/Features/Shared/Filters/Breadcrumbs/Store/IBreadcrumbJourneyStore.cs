using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Store;

public interface IBreadcrumbJourneyStore
{
    BreadcrumbJourney Load();
    void Save(BreadcrumbJourney journey);
    void Clear();
}
