using System.ComponentModel.DataAnnotations;

namespace DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests.Options;

public sealed class AccessibilityTestOptions
{
    public string DefaultMinimumWcag { get; set; } = string.Empty;
    public Dictionary<string, AccessibilityTest> Tests { get; set; } = [];
}

public sealed class AccessibilityTest
{
    [Required]
    [MinLength(1)]
    public string Route { get; set; } = string.Empty;
}