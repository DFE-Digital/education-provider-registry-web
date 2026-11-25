using DfE.Document.Abstractions;

namespace DfE.EducationProviderRegistry.Web.PageComponents;

public sealed class TextComponent
{
    public TextContent GetContent(IElement context)
    {
        ArgumentNullException.ThrowIfNull(context);

        IElement? link = context.Find("a");

        TextContent output = new()
        {
            Text = context.Text.Trim() ?? string.Empty,
            Link =
                link is null ?
                    null :
                        new Link(
                            url: link?.Attribute("href") ?? throw new ArgumentException("Could not find href attribute on link"),
                            securityAttributes: link.Attribute("rel")?.Split(" ") ?? [],
                            opensInNewWindow: link.Attribute("target")?.Equals("_blank") ?? false)
        };

        return output;
    }
}
