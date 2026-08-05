namespace DfE.WebDriver.WebDriver;

internal readonly struct BrowserViewport
{
    public BrowserViewport(int width, int height)
    {
        if (width < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width), "Width cannot be negative.");
        }
        if (height < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width), "Width cannot be negative.");
        }

        Width = width;
        Height = height;
    }

    public int Width { get; }
    public int Height { get; }
}
