using DfE.EducationProviderRegistry.Web.Mvc.Controllers;
using System.Reflection;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Controllers.TestDoubles;

/// <summary>
/// Extracts the JSON portion from a Set-Cookie header.
/// Example header:
/// cookies_policy={"analytics":true}; path=/; expires=...
/// </summary>
public static class CookiesControllerInvokerTestDouble
{
    public static bool? InvokeReadAnalyticsConsent(CookiesController controller)
    {
        ArgumentNullException.ThrowIfNull(controller);

        MethodInfo method =
            controller
                .GetType()
                .GetMethod("ReadAnalyticsConsent", BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new InvalidOperationException("ReadAnalyticsConsent method not found.");

        object? result = method.Invoke(controller, null);
        return result as bool?;
    }
}
