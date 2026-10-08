using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Breadcrumbs.Models;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Breadcrumbs.Stores;

public interface IBreadcrumbJourneyStore
{
    BreadcrumbJourney Get();
    void Save(BreadcrumbJourney journey);
    void Clear();
}
