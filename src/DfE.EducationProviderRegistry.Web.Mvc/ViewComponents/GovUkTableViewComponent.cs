using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;

public class GovUkTableViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(GovUkTable model)
    {
        return View(model);
    }
}