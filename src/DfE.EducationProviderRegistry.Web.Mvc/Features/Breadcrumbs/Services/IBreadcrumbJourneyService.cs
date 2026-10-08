using DfE.EducationProviderRegistry.Web.Mvc.Features.Breadcrumbs.Models;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Breadcrumbs.Services;

public interface IBreadcrumbJourneyService
{
    BreadcrumbJourney Get();

    void StartSearchJourney(string searchResultsUrl);

    void VisitEstablishment(string name, string url);

    void VisitGroup(string name, string url);

    void Clear();
}
