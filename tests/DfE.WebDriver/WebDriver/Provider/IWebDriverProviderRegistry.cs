namespace DfE.WebDriver.WebDriver.Provider;

internal interface IWebDriverProviderRegistry
{
    IWebDriverProvider GetProvider(string key);
}
