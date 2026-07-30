using OpenQA.Selenium;

namespace DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests.Actions;

public sealed class AccessibilityScanContext
{
    public required IWebDriver WebDriver { get; init; }
    public required Uri BaseUri { get; init; }
    public required AccessibilityScanAction Action { get; init; }
    public required CancellationToken CancellationToken { get; init; }
}