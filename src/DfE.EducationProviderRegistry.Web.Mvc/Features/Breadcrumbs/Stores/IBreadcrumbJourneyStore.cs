using DfE.EducationProviderRegistry.Web.Mvc.Features.Breadcrumbs.Models;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Breadcrumbs.Stores;

// INTERFACES
public interface IBreadcrumbJourneyStore
{
    BreadcrumbJourney Get();
    void Save(BreadcrumbJourney journey);
    void Clear();
}
