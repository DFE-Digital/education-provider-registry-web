using OpenQA.Selenium.Chrome;

namespace BrowserLibrary.WebDriver.Chrome;

internal sealed class ChromeOptionsFactory : IWebDriverOptionsFactory<ChromeOptions>
{
    public ChromeOptions CreateOptions(WebDriverSessionRequest spec)
    {
        ChromeOptions options = new();
        // TODO back into SessionSpec as options so other browsers consume
        options.AddArguments(
            "--incognito",
            // https://github.com/SeleniumHQ/selenium/issues/6049 observed on ubuntu 22.04 runners
            "--disable-dev-shm-usage",
            "--disable-gpu",
            // screen size setting
            "--window-size=1920,1080",
            "--start-maximized",
            "--start-fullscreen",
            // Bypass localhost certificate errors in CI
            "--allow-insecure-localhost");

        if (spec.TryGet(WebDriverSessionContextKeys.Headless,
                out bool headless)
                    && headless)
        {
            // see https://www.selenium.dev/blog/2023/headless-is-going-away/
            options.AddArgument("--headless=new");
            return options;
        }

        return options;
    }
}