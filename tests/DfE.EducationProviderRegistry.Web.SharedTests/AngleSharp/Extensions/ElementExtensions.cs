using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using HttpMethod = System.Net.Http.HttpMethod;

namespace DfE.EducationProviderRegistry.Web.SharedTests.AngleSharp.Extensions;

public static class ElementExtensions
{
    public static (HttpMethod method, string? action) ParseFormAttributes(this IElement formElement)
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