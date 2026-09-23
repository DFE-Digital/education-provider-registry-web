using Microsoft.AspNetCore.Mvc.Razor;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Datasets.Views;

public sealed class DownloadDatasetsViewLocationExpander : IViewLocationExpander
{
    public void PopulateValues(ViewLocationExpanderContext context){
    }

    public IEnumerable<string> ExpandViewLocations(
        ViewLocationExpanderContext context,
        IEnumerable<string> viewLocations)
    {
        string controller = context.ActionContext.ActionDescriptor.RouteValues["controller"]!;

        return new[]
        {
            $"/Features/Download/Datasets/Views/{controller}/{{0}}.cshtml",
            $"/Features/Download/Datasets/Views/Shared/{{0}}.cshtml"
        }.Concat(viewLocations);
    }
}