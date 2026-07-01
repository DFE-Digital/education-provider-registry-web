using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.ViewComponents.Table;

public class GovUkTableViewComponent : ViewComponent
{
    public Task<IViewComponentResult> InvokeAsync(GovUkTable model)
    {
        return Task.FromResult(
            View(
                viewName: "Default",
                model: model) as IViewComponentResult);
    }
}