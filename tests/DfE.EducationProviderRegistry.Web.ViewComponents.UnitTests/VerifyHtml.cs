namespace DfE.EducationProviderRegistry.Web.ViewComponents.UnitTests;

public static class HtmlMarkupVerify
{
    public static async Task VerifyAsync(
        string testName,
        string viewPath,
        object? viewModel,
        string? outputArtifactDirectory = null)
    {
        using ViewComponentRenderer renderer = new();

        string html = await renderer.RenderAsync(
            viewPath,
            viewModel);

        VerifySettings settings = new();

        settings
            .UseFileName(testName);

        settings
            .UseDirectory(string.IsNullOrWhiteSpace(outputArtifactDirectory) ? "snapshots" : outputArtifactDirectory);

        await Verifier.Verify(
            target: FormatHtml(html),
            extension: "html",
            settings);
    }

    private static string FormatHtml(string html)
    {
        return html
            .Replace("\r\n", "\n")
            .Trim();
    }
}