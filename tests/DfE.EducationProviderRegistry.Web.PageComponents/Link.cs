namespace DfE.EducationProviderRegistry.Web.PageComponents;

public sealed record class Link
{
    public Link(string? url, IReadOnlyCollection<string>? securityAttributes = null, bool opensInNewWindow = false)
    {
        Url = url ?? string.Empty;
        SecurityAttributes = securityAttributes ?? [];
        OpensInNewWindow = opensInNewWindow;
    }

    public string Url { get; }
    public IReadOnlyCollection<string> SecurityAttributes { get; }
    public bool OpensInNewWindow { get; }
}
