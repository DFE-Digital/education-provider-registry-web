using Microsoft.AspNetCore.Mvc.Rendering;

namespace DfE.EducationProviderRegistry.Web.Mvc.Extensions
{
    public static class HtmlExtensions
    {
        public static string ActiveClassForPath(this IHtmlHelper html, string routePrefix)
        {
            var path = html.ViewContext.HttpContext.Request.Path.Value?.ToLower() ?? "";
            routePrefix = routePrefix.ToLower();

            if (path == routePrefix || path.StartsWith(routePrefix + "/"))
            {
                return "govuk-service-navigation__item--active";
            }

            return string.Empty;
        }
    }
}
