namespace BrowserLibrary.Public.Session;

public interface IWebDriverSessionBuilder
{
    IWebDriverSessionBuilder WithBrowser(string type);
    IWebDriverSessionBuilder WithChrome();
    IWebDriverSessionBuilder WithEdge();
    IWebDriverSessionBuilder WithFirefox();
    IWebDriverSessionBuilder WithStartMaximised(bool maximised);
    IWebDriverSessionBuilder WithBrowserVersion(string version);
    IWebDriverSessionBuilder WithHeadless(bool headless);
    IWebDriverSessionBuilder WithViewport(int width, int height);
    IWebDriverSessionBuilder WithAllowInsecureLocalConnections(bool insecure);
    IWebDriverSession Build();
}