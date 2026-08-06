using DfE.WebDriver.Public.Session;
using DfE.WebDriver.WebDriver.Options;
using DfE.WebDriver.WebDriver.Provider;
using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace DfE.WebDriver;

public static class CompositionRoot
{
    public static IServiceCollection AddWebDriver(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentException("Services cannot be null.", nameof(services));
        }

        // Client IWebDriverSessionBuilder
        services.AddTransient<IWebDriverSessionBuilder, WebDriverSessionBuilder>();

        // DriverOptions
        services.AddScoped<IWebDriverOptionsFactory<ChromeOptions>, ChromeOptionsFactory>();
        services.AddScoped<IWebDriverOptionsFactory<FirefoxOptions>, FirefoxOptionsFactory>();

        // DriverProvider
        services.AddScoped<IWebDriverProviderRegistry>((sp) =>
        {
            Dictionary<string, Func<IWebDriverProvider>> webDriverProviders = new()
            {
                { "chrome",
                    () => new ChromeDriverProvider(
                        sp.GetRequiredService<IWebDriverOptionsFactory<ChromeOptions>>()) },
                { "firefox",
                    () => new FirefoxDriverProvider(
                        sp.GetRequiredService<IWebDriverOptionsFactory<FirefoxOptions>>()) },
                //{ "edge", () => new EdgeWebDriverProvider() }
            };
            return new WebDriverProviderRegistry(webDriverProviders);
        });

        return services;
    }
}
