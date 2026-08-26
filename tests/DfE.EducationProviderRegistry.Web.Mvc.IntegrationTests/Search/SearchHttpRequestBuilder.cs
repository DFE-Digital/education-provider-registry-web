namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Search;

public sealed class SearchHttpRequestBuilder
{
    private readonly SearchPanel _searchPanel;
    private string? _identityTerm;
    private string? _locationTerm;

    public SearchHttpRequestBuilder(SearchPanel searchPanel)
    {
        _searchPanel = searchPanel;
    }

    public SearchHttpRequestBuilder WithIdentitySearchTerm(string value)
    {
        _identityTerm = value;
        return this;
    }

    public SearchHttpRequestBuilder WithLocationTerm(string value)
    {
        _locationTerm = value;
        return this;
    }

    public HttpRequestMessage Build()
    {
        Dictionary<string, string> values = [];

        if (_identityTerm is not null)
        {
            values.Add(_searchPanel.GetIdentityInputName(), _identityTerm);
        }

        if (_locationTerm is not null)
        {
            values.Add(_searchPanel.GetLocationInputName(), _locationTerm);
        }

        (HttpMethod method, Uri target) = _searchPanel.GetFormDetails();

        return new HttpRequestMessage(method, target)
        {
            Content = new FormUrlEncodedContent(values)
        };
    }

    public static SearchHttpRequestBuilder Create(SearchPanel panel) => new(panel);
}