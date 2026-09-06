using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Extensions;

internal static class ElementExtensions
{
    internal static (HttpMethod method, string? action) ParseFormAttributes(this IElement formElement)
    {
        bool isFormTag = formElement.TagName.Equals(TagNames.Form, StringComparison.OrdinalIgnoreCase);

        if (!isFormTag)
        {
            throw new ArgumentException("Element being parsed is not a form element");
        }

        IHtmlFormElement form = (IHtmlFormElement)formElement;

        HttpMethod method =
            formElement.GetAttribute("method")?.ToLowerInvariant() ==
                HttpMethod.Get.Method ?
                    HttpMethod.Get : HttpMethod.Post ??
                        HttpMethod.Get;

        return (method, form.Action);
    }
}