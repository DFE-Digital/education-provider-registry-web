namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.NavigationEvaluators;

public interface INavigationEvaluator
{
    bool IsDirectNavigation(HttpContext context);
}
