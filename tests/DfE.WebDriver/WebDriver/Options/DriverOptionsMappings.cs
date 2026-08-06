using OpenQA.Selenium;

namespace DfE.WebDriver.WebDriver.Options;

internal static class DriverOptionsMappings
{
    public static IEnumerable<KeyValuePair<string, Action<WebDriverSessionRequest, TOptions>>> CreateSharedMappings<TOptions>()
            where TOptions : DriverOptions
    {
        yield return new(
            WebDriverSessionRequestKeys.BrowserVersion,
            (req, options) =>
            {
                options.BrowserVersion = req.BrowserVersion;
            });

        yield return new(
            WebDriverSessionRequestKeys.AllowInsecureCertificates,
            (req, options) =>
            {
                options.AcceptInsecureCertificates = true;
            });
    }
}