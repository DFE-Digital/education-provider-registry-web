namespace DfE.WebDriver.WebDriver;

internal interface IWebDriverOptionsFactory<out TOptions>
{
    TOptions CreateOptions(WebDriverSessionRequest spec);
}
