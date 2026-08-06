using DfE.WebDriver.WebDriver.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace DfE.WebDriver.WebDriver.Provider;

internal class FirefoxDriverProvider : IWebDriverProvider
{
    private readonly IWebDriverOptionsFactory<FirefoxOptions> _optionsFactory;

    public FirefoxDriverProvider(IWebDriverOptionsFactory<FirefoxOptions> optionsFactory)
    {
        _optionsFactory = optionsFactory ?? throw new ArgumentNullException(nameof(optionsFactory));
    }

    IWebDriver IWebDriverProvider.GetDriver(WebDriverSessionContext spec)
    {
        FirefoxDriverService service = FirefoxDriverService.CreateDefaultService();
        FirefoxOptions options = _optionsFactory.CreateOptions(spec);
        return new FirefoxDriver(service, options);
    }
}
