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

        if (!string.IsNullOrWhiteSpace(searchRequest.What))
        {
            query.Add(
                nameof(searchRequest.What),
                searchRequest.What);
        }

        if (!string.IsNullOrWhiteSpace(searchRequest.Where))
        {
            query.Add(
                nameof(searchRequest.Where),
                searchRequest.Where);
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
