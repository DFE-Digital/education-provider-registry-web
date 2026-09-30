using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.ViewComponents.Table;

public class GovUkTableViewComponent : ViewComponent
{
    public Task<IViewComponentResult> InvokeAsync(GovUkTable model)
    {
        return Task.FromResult(
            View(
                "/Views/Shared/Components/GovUkTable/Default.cshtml",
                model) as IViewComponentResult);
    }
}
