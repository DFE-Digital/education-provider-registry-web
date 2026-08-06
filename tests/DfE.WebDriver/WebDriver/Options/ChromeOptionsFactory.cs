using OpenQA.Selenium.Chrome;

namespace DfE.WebDriver.WebDriver.Options;

internal sealed class ChromeOptionsFactory
    : WebDriverOptionsFactory<ChromeOptions>
{
    private static readonly IReadOnlyDictionary<string, Action<WebDriverSessionRequest, ChromeOptions>> Handlers =
        new Dictionary<
            string,
            Action<WebDriverSessionRequest, ChromeOptions>>
        {
            [WebDriverSessionRequestKeys.Headless] =
                (session, options) =>
                {
                    if (session.Headless)
                    {
                        options.AddArgument("--headless=new");
                    }
                },
            [WebDriverSessionRequestKeys.StartMaximised] =
                (session, options) =>
                {
                    if (session.StartMaximised)
                    {
                        options.AddArgument("--start-maximized");
                        options.AddArgument("--start-fullscreen");
                    }

                },
            [WebDriverSessionRequestKeys.Viewport] =
                (session, options) =>
                {
                    if (!session.Viewport.HasValue)
                    {
                        return;
                    }
                    BrowserViewport viewport = session.Viewport.Value;
                    options.AddArgument($"--window-size={viewport.Width},{viewport.Height}");
                },
            [WebDriverSessionRequestKeys.AllowInsecureCertificates] =
                (session, options) =>
                {
                    options.AddArgument("--allow-insecure-localhost");
                }
        };

    public ChromeOptionsFactory()
        : base(Handlers)
    {
    }

    protected override void ConfigureOptions(ChromeOptions options)
    {
        options.AddArguments(
            "--incognito",
            // https://github.com/SeleniumHQ/selenium/issues/6049 observed on ubuntu 22.04 runners
            "--disable-dev-shm-usage",
            "--disable-gpu");
    }
}