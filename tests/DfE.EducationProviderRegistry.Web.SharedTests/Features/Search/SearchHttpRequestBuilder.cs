using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using System.Text;
using HttpMethod = System.Net.Http.HttpMethod;

namespace DfE.EducationProviderRegistry.Web.SharedTests.Features.Search;

public sealed class SearchHttpRequestBuilder
{
    private Uri? _uri;
    private HttpMethod _method = HttpMethod.Post;
    private string? _sortDirection;
    private readonly List<KeyValuePair<string, string[]>> _filters;
    private string? _identityTerm;
    private string? _locationTerm;
    private int? _page;

    public SearchHttpRequestBuilder()
    {
        _filters = [];
    }

    public SearchHttpRequestBuilder WithBaseUri(Uri? uri)
    {
        _uri = uri;
        return this;
    }

    public SearchHttpRequestBuilder WithMethod(HttpMethod method)
    {
        _method = method;
        return this;
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

    public SearchHttpRequestBuilder WithSortDirection(string sortDirection)
    {
        _sortDirection = sortDirection;
        return this;
    }

    public SearchHttpRequestBuilder WithFilter(string facet, string[] values)
    {
        _filters.Add(new(facet, values));
        return this;
    }

    public SearchHttpRequestBuilder WithPage(int page)
    {
        _page = page;
        return this;
    }

    public HttpRequestMessage Build()
    {
        Dictionary<string, string> values = [];

        if (_identityTerm is not null)
        {
            values.Add(nameof(SearchRequestViewModel.SearchKeywords), _identityTerm);
        }

        if (_locationTerm is not null)
        {
            values.Add(nameof(SearchRequestViewModel.Address), _locationTerm);
        }

        if (_uri is null)
        {
            throw new ArgumentException("Uri cannot be null");
        }

        UriBuilder uriBuilder = new()
        {
            Scheme = _uri.Scheme,
            Host = _uri.Host,
            Path = "/search/results",
            Port = _uri.Port,
            Query = CreateQueryString(_sortDirection, _filters, _page)
        };

        return new HttpRequestMessage(_method, uriBuilder.Uri)
        {
            Content = new FormUrlEncodedContent(values)
        };
    }

    private static string CreateQueryString(string? sort, IEnumerable<KeyValuePair<string, string[]>> filters, int? page = null)
    {
        StringBuilder queryStringBuilder = new();

        if (!string.IsNullOrWhiteSpace(sort))
        {
            queryStringBuilder.Append($"sort={sort}");
        }

        if (page is not null)
        {
            queryStringBuilder.Append(queryStringBuilder.Length > 0 ? "&" : string.Empty);
            queryStringBuilder.Append($"pageNumber={page.Value}");
        }

        return
            filters
                .SelectMany((filter) =>
                    filter.Value.Select((value) =>
                        $"{Uri.EscapeDataString($"SelectedFacets[{filter.Key}]")}" +
                        $"=" +
                        $"{Uri.EscapeDataString(value)}"))
            .Aggregate(
                seed: queryStringBuilder,
                func: (sb, value) => sb.Append(value).Append('&'),
                resultSelector: (sb) => sb.ToString().TrimEnd('&'))
            .ToString();
    }

    public static SearchHttpRequestBuilder Create() => new();
}

public enum SortDirection
{
    Ascending,
    Descending
}