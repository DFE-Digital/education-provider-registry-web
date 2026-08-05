using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace DfE.WebDriver.WebDriver.Chrome;

internal sealed class ChromeDriverProvider : IWebDriverProvider
{
    private readonly IWebDriverOptionsFactory<ChromeOptions> _optionsFactory;

    public ChromeDriverProvider(
        IWebDriverOptionsFactory<ChromeOptions> optionsFactory)
    {
        _optionsFactory = optionsFactory;
    }

    IWebDriver IWebDriverProvider.GetDriver(WebDriverSessionRequest spec)
    {
        ChromeDriverService service = ChromeDriverService.CreateDefaultService();
        ChromeOptions options = _optionsFactory.CreateOptions(spec);
        return new ChromeDriver(service, options);
    }
}
