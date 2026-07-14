using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace DfE.EducationProviderRegistry.Web.Mvc.Extensions;

public static class ModelStateExtensions
{
    public static bool HasErrorFor(this ViewDataDictionary viewData, string key)
    {
        return viewData.ModelState.TryGetValue(key, out ModelStateEntry? entry)
            && entry.Errors.Count > 0;
    }
}