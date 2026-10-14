namespace DfE.WebDriver.Public.Session.Extensions;

public static class WebDriverSessionBuilderExtensions
{
    public static IWebDriverSessionBuilder WithChrome(this IWebDriverSessionBuilder builder)
    {
        return builder.WithBrowser("chrome");
    }

    public static IWebDriverSessionBuilder WithEdge(this IWebDriverSessionBuilder builder)
    {
        return builder.WithBrowser("edge");
    }

    public static IWebDriverSessionBuilder WithFirefox(this IWebDriverSessionBuilder builder)
    {
        return builder.WithBrowser("firefox");
    }
}
