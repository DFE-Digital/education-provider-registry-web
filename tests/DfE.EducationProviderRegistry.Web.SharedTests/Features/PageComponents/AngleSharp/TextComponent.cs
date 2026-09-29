using AngleSharp.Dom;

namespace DfE.EducationProviderRegistry.Web.SharedTests.Features.PageComponents.AngleSharp;

public sealed record class TextComponent
{
    public TextContent GetContent(IElement context)
    {
        ArgumentNullException.ThrowIfNull(context);

        IElement? link = context.QuerySelector("a");

        TextContent output = new()
        {
            Text = context.Text().Trim(),
            Link =
                link is null ?
                    null :
                        new Link(
                            url: link?.GetAttribute("href") ?? throw new ArgumentException("Could not find href attribute on link"),
                            securityAttributes: link.GetAttribute("rel")?.Split(" ") ?? [],
                            opensInNewWindow: link.GetAttribute("target")?.Equals("_blank") ?? false)
        };

        return output;
    }
}
