using System.ComponentModel.DataAnnotations;

namespace DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests.Options;

public sealed class AccessibilityTestOptions
{
    public string ArtifactOutputDirectory { get; set; } = "artifacts";
    public string[]? WcagTags { get; set; }
    public Dictionary<string, AccessibilityTest> Scans { get; set; } = [];
}

public sealed class AccessibilityTest
{
    [Required]
    [MinLength(1)]
    public string Route { get; set; } = string.Empty;
}