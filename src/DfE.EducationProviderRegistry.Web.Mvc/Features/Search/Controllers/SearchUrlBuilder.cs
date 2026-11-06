using System.Globalization;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Controllers;

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

        AddFilters(query, searchRequest.Filters);

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

    private static void AddFilters(
        QueryBuilder query,
        Dictionary<string, FilterInputViewModel> filters)
    {
        foreach ((string? name, FilterInputViewModel? filter) in filters)
        {
            if (filter.DateRange is { IsEmpty: false } dateRange)
            {
                string prefix = $"Filters[{name}].DateRange";
                AddDateRangeInput(query, $"{prefix}.From", dateRange.From);
                AddDateRangeInput(query, $"{prefix}.To", dateRange.To);
            }

            if (filter.NumberRange is { IsEmpty: false } ageRange)
            {
                string prefix = $"Filters[{name}].NumberRange";
                if (ageRange.From is int from)
                {
                    query.Add($"{prefix}.From", from.ToString(CultureInfo.InvariantCulture));
                }
                if (ageRange.To is int to)
                {
                    query.Add($"{prefix}.To", to.ToString(CultureInfo.InvariantCulture));
                }
            }
            // Add other filter types here as needed
        }
    }

    private static void AddDateRangeInput(QueryBuilder query, string prefix, DateInputViewModel? date)
    {
        if (date is null)
            return;

        query.Add($"{prefix}.Day", date.Day ?? string.Empty);
        query.Add($"{prefix}.Month", date.Month ?? string.Empty);
        query.Add($"{prefix}.Year", date.Year ?? string.Empty);
    }
}
