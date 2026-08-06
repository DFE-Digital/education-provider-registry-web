namespace DfE.WebDriver.WebDriver;

internal sealed record WebDriverSessionRequest
{
    public WebDriverSessionRequest(
        WebDriverSessionContext request)
    {
        Headless = true;
        StartMaximised = true;

        if (request.TryGet<bool>(WebDriverSessionRequestKeys.Headless, out bool headless))
        {
            Headless = headless;
        }

        if (request.TryGet<bool>(WebDriverSessionRequestKeys.StartMaximised, out bool maximised))
        {
            StartMaximised = maximised;
        }

        BrowserVersion =
            request.TryGet<string>(
                WebDriverSessionRequestKeys.BrowserVersion,
                out string? version)
                    ? version
                    : null;

        Viewport =
            request.TryGet<BrowserViewport>(
                WebDriverSessionRequestKeys.Viewport,
                out BrowserViewport viewport)
                    ? viewport
                    : null;
    }

    public bool Headless { get; }
    public bool StartMaximised { get; }
    public string? BrowserVersion { get; }
    public BrowserViewport? Viewport { get; }
}