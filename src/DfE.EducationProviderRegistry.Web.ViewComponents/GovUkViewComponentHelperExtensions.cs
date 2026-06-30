using DfE.EducationProviderRegistry.Web.ViewComponents.Table;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.ViewComponents;

public static class GovUkViewComponentHelperExtensions
{
    public static Task<IHtmlContent> Table(
        this IViewComponentHelper helper, GovUkTable model) 
            => helper.InvokeAsync(nameof(GovUkTable), model);
}
