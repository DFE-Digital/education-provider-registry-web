using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.ViewComponents.SummaryList;

public class GovUkSummaryListViewComponent : ViewComponent
{
    public Task<IViewComponentResult> InvokeAsync(GovUkSummaryList model)
    {
        return Task.FromResult(
            View(
                viewName: "/Views/Shared/Components/GovUkSummaryList/Default.cshtml",
                model: model) as IViewComponentResult);
    }
}