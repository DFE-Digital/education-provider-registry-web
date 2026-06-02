using DfE.EducationProviderRegistry.Web.Mvc.ViewModels.Components;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;

public class EstablishmentSearchResultViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(EstablishmentSearchResultViewModel model)
    {
        return View(model);
    }
}
