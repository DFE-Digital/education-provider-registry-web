using DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests.Actions;

namespace DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests.Options;

public sealed class AccessibilityTestOptions
{
    public string ArtifactsOutputDirectory { get; set; } = "artifacts";
    public string[]? WcagTags { get; set; }
    public Dictionary<string, AccessibilityTest> Scans { get; set; } = [];
}

public sealed class AccessibilityTest
{
    public IReadOnlyList<AccessibilityScanAction> Actions { get; init; } = [];
}