namespace DfE.WebDriver.WebDriver;

internal interface IWebDriverProviderRegistry
{
    IWebDriverProvider GetProvider(string key);
}
