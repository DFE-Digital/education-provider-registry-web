using HttpMethod = System.Net.Http.HttpMethod;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness.Anglesharp;

internal sealed record HtmlForm(
    Uri Action,
    HttpMethod Method,
    string? Enctype,
    IReadOnlyDictionary<string, string> Fields)
{
    public HtmlForm AddField(
        string name,
        string value)
    {
        Dictionary<string, string> fields = new(Fields)
        {
            [name] = value
        };

        return this with
        {
            Fields = fields
        };
    }
}