using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search;

public static class SearchUrlBuilder
{
    public static string BuildPageUrl(
        IUrlHelper urlHelper,
        SearchRequestViewModel searchRequest,
        int pageNumber)
    {
        ArgumentNullException.ThrowIfNull(urlHelper);
        ArgumentNullException.ThrowIfNull(searchRequest);

        QueryBuilder query = new();

        if (!string.IsNullOrWhiteSpace(searchRequest.SearchKeywords))
        {
            query.Add(
                nameof(searchRequest.SearchKeywords),
                searchRequest.SearchKeywords);
        }

        if (!string.IsNullOrWhiteSpace(searchRequest.Address))
        {
            query.Add(
                nameof(searchRequest.Address),
                searchRequest.Address);
        }

        if (!string.IsNullOrWhiteSpace(searchRequest.Sort))
        {
            query.Add(
                nameof(searchRequest.Sort),
                searchRequest.Sort);
        }

        query.Add(
            nameof(searchRequest.PageNumber),
            pageNumber.ToString());

        query.Add(
            nameof(searchRequest.RecordsPerPage),
            searchRequest.RecordsPerPage.ToString());

        AddSelectedFacets(
            query,
            searchRequest.SelectedFacets);

        string path = urlHelper.Action(
            "Search",
            "Search")!;

        return path + query.ToQueryString();
    }

    private static void AddSelectedFacets(
        QueryBuilder query,
        Dictionary<string, List<string>>? selectedFacets)
    {
        if (selectedFacets is null)
        {
            return;
        }

        foreach (KeyValuePair<string, List<string>> facet in selectedFacets)
        {
            foreach (string value in facet.Value)
            {
                query.Add(
                    $"SelectedFacets[{facet.Key}]",
                    value);
            }
        }
    }
}