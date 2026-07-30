namespace DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests.Actions;

public sealed class AccessibilityScanAction
{
    public string Name { get; init; } = string.Empty;

    public Dictionary<string, string> Options { get; init; } = [];

    public string GetRequiredOption(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        string normalised = key.Trim().ToLowerInvariant();

        if (!Options.TryGetValue(normalised, out string? value))
        {
            throw new InvalidOperationException(
                $"Missing option '{normalised}'.");
        }

        return value;
    }
}