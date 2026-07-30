namespace DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests.Actions;

internal sealed class NavigateActionHandler : IAccessibilityScanActionHandler
{
    public async Task ExecuteAsync(AccessibilityScanContext context)
    {
        string route = context.Action.GetRequiredOption("Route");

        Uri uri = new(baseUri: context.BaseUri, relativeUri: route);

        await context.WebDriver.Navigate().GoToUrlAsync(uri);
    }
}