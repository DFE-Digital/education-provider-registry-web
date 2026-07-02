using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.ViewComponents.Table;

public class SharedGovUkTableViewComponent : ViewComponent
{
    public Task<IViewComponentResult> InvokeAsync(GovUkTable model)
    {
        return Task.FromResult(
            View(
                "/Views/Shared/Components/SharedGovUkTable/Default.cshtml",
                model) as IViewComponentResult);
    }
}
