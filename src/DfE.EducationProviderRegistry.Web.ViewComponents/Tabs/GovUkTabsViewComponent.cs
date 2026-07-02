using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.ViewComponents.Tabs;

public class GovUkTabsViewComponent : ViewComponent
{
    public Task<IViewComponentResult> InvokeAsync(GovUkTabs model)
    {
        return Task.FromResult(
            View(
                "/Views/Shared/Components/GovUkTabs/Default.cshtml",
                model) as IViewComponentResult);
    }
}
