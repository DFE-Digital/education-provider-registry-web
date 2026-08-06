using DfE.WebDriver.Public.Session;
using DfE.WebDriver.WebDriver.Options;
using DfE.WebDriver.WebDriver.Provider;
using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium.Chrome;

namespace DfE.WebDriver;

public static class CompositionRoot
{
    public static IServiceCollection AddWebDriver(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentException("Services cannot be null.", nameof(services));
        }

        // Client SessionBuilder
        services.AddTransient<IWebDriverSessionBuilder, WebDriverSessionBuilder>();

        // DriverOptions
        services.AddScoped<WebDriverOptionsFactory<ChromeOptions>, ChromeOptionsFactory>();

        // DriverProvider
        services.AddScoped<IWebDriverProviderRegistry>((sp) =>
        {
            Dictionary<string, Func<IWebDriverProvider>> webDriverProviders = new()
            {
                { "chrome",
                    () => new ChromeDriverProvider(
                        sp.GetRequiredService<WebDriverOptionsFactory<ChromeOptions>>()) },
                //{ "firefox", () => new FirefoxWebDriverProvider() },
                //{ "edge", () => new EdgeWebDriverProvider() }
            };
            return new WebDriverProviderRegistry(webDriverProviders);
        });

        return services;
    }
}
