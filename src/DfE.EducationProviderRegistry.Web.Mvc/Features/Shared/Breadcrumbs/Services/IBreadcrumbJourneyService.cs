using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Breadcrumbs.Models;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Breadcrumbs.Services;

public interface IBreadcrumbJourneyService
{
    BreadcrumbJourney Get();

    void StartSearchJourney(string searchResultsUrl);

    void NavigateTo(BreadcrumbItem destination);

    void Clear();
}
