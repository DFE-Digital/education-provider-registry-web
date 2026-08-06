using DfE.WebDriver.WebDriver.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace DfE.WebDriver.WebDriver.Provider;

internal sealed class ChromeDriverProvider : IWebDriverProvider
{
    private readonly WebDriverOptionsFactory<ChromeOptions> _optionsFactory;

    public ChromeDriverProvider(WebDriverOptionsFactory<ChromeOptions> optionsFactory)
    {
        _optionsFactory = optionsFactory ?? throw new ArgumentNullException(nameof(optionsFactory));
    }

    IWebDriver IWebDriverProvider.GetDriver(WebDriverSessionContext spec)
    {
        ChromeDriverService service = ChromeDriverService.CreateDefaultService();
        ChromeOptions options = _optionsFactory.CreateOptions(spec);
        return new ChromeDriver(service, options);
    }
}
