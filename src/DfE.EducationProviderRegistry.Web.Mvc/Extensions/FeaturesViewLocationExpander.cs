using Microsoft.AspNetCore.Mvc.Razor;

namespace DfE.EducationProviderRegistry.Web.Mvc.Extensions;

public class FeatureViewLocationExpander : IViewLocationExpander
{
    public void PopulateValues(ViewLocationExpanderContext context)
    {
        // Optional: store values for later use
        // Not required for simple scenarios
    }

    public IEnumerable<string> ExpandViewLocations(
        ViewLocationExpanderContext context,
        IEnumerable<string> viewLocations)
    {
        string? controller = context.ActionContext.RouteData.Values["controller"]?.ToString();

        if (string.IsNullOrEmpty(controller))
        {
            return viewLocations;
        }

        List<string> locations =
    [
        $"/Features/{controller}/Views/{{1}}/{{0}}.cshtml",
        $"/Features/{controller}/Views/Shared/{{0}}.cshtml",
        
        // Explicit fallback
        "/Views/Shared/{0}.cshtml",

        .. viewLocations,
    ];

        return locations;
    }
}