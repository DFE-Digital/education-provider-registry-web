using BrowserLibrary.Public.Session;
using BrowserLibrary.WebDriver;
using BrowserLibrary.WebDriver.Chrome;
using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium.Chrome;

namespace BrowserLibrary;

public static class CompositionRoot
{
    public static IServiceCollection AddWebDriver(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentException("Services cannot be null.", nameof(services));
        }

        services.AddTransient<IWebDriverSessionBuilder, WebDriverSessionBuilder>();

        // TODO options to template to share parsing of spec. share SessionSpec parsing TOptions CreateOptions(SessionSpec) ApplyHeadless(bool);
        services.AddTransient<IWebDriverOptionsFactory<ChromeOptions>, ChromeOptionsFactory>();
        services.AddScoped<IWebDriverProviderRegistry>((sp) =>
        {
            Dictionary<string, Func<IWebDriverProvider>> webDriverProviders = new Dictionary<string, Func<IWebDriverProvider>>()
            {
                { "chrome", () => new ChromeDriverProvider(sp.GetRequiredService<IWebDriverOptionsFactory<ChromeOptions>>()) },
                //{ "firefox", () => new FirefoxWebDriverProvider() },
                //{ "edge", () => new EdgeWebDriverProvider() }
            };
            return new WebDriverProviderRegistry(webDriverProviders);
        });

        return services;
    }
}
