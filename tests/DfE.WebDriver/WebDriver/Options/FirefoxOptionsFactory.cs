using OpenQA.Selenium.Firefox;

namespace DfE.WebDriver.WebDriver.Options;

internal sealed class FirefoxOptionsFactory : WebDriverOptionsFactory<FirefoxOptions>
{
    private static readonly IEnumerable<KeyValuePair<string, Action<WebDriverSessionRequest, FirefoxOptions>>> DefaultHandlers =
        [
            new(
                WebDriverSessionRequestKeys.Headless,
                (req, options) =>
                {
                    if (req.Headless)
                    {
                        options.AddArgument("--headless");
                    }
                }),
            new(
                WebDriverSessionRequestKeys.StartMaximised,
                (req, options) =>
                {
                    if (req.StartMaximised)
                    {
                        options.AddArgument("--maximized");
                    }
                }),
            new(
                WebDriverSessionRequestKeys.Viewport,
                (req, options) =>
                {
                    if (req.Viewport.HasValue)
                    {
                        BrowserViewport viewport = req.Viewport.Value;
                        options.AddArguments($"--width={viewport.Width}", $"--height={viewport.Height}");
                    }
                })
        ];
    public FirefoxOptionsFactory() : base(DefaultHandlers)
    {
    }

    protected override void ConfigureOptions(FirefoxOptions options)
    {

    }
}
