namespace DfE.EducationProviderRegistry.Web.SharedTests.Features.PageComponents;
// TODO default trim content options and new line?
public sealed record class TextContent
{
    private string _text;

    public TextContent()
    {
        _text = string.Empty;
    }

    public string Text
    {
        get => _text;
        set => _text = value ?? string.Empty;
    }

    public Link? Link { get; set; }

    public override string ToString() => _text;
}
