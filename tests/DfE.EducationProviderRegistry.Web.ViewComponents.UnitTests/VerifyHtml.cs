namespace DfE.EducationProviderRegistry.Web.ViewComponents.UnitTests;

public static class VerifyHtml
{
    public static Task Verify(string html)
    {
        VerifySettings settings = new();

        html = FormatHtml(html);

        return Verifier.Verify(html, extension: "html", settings);
    }

    private static string FormatHtml(string html)
    {
        return html
            .Replace("\r\n", "\n")
            .Trim();
    }
}