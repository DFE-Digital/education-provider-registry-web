using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.ViewComponents.Paragraph;

public class ParagraphViewComponent : ViewComponent
{
    public Task<IViewComponentResult> InvokeAsync(Paragraph model)
    {
        return Task.FromResult(
            View(
                viewName: "/Views/Shared/Components/Paragraph/Default.cshtml",
                model: model) as IViewComponentResult);
    }
}
