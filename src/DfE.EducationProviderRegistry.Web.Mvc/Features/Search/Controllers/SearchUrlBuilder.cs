using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.Request;
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

        QueryBuilder query = [];

        AddIfNotEmpty(
            query,
            nameof(searchRequest.What),
            searchRequest.What);

        AddIfNotEmpty(
            query,
            nameof(searchRequest.Where),
            searchRequest.Where);

        AddIfNotEmpty(
            query,
            nameof(searchRequest.Sort),
            searchRequest.Sort);

        query.Add(
            nameof(searchRequest.PageNumber),
            pageNumber.ToString());

        query.Add(
            nameof(searchRequest.RecordsPerPage),
            searchRequest.RecordsPerPage.ToString());

        AddSelectedFacets(
            query,
            searchRequest.SelectedFacets);

        AddFilters(
            query,
            searchRequest.Filters);

        string path = urlHelper.Action(action: "Search", controller: "Search")!;

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
            foreach (string? value in facet.Value)
            {
                AddIfNotEmpty(
                    query,
                    $"SelectedFacets[{facet.Key}]",
                    value);
            }
        }
    }

    private static void AddFilters(
        QueryBuilder query,
        Dictionary<string, FilterInputViewModel> filters)
    {
        foreach ((string name, FilterInputViewModel filter) in filters)
        {
            if (filter.DateRange is { IsEmpty: false } dateRange)
            {
                string prefix =
                    $"Filters[{name}].DateRange";

                AddDateRangeInput(
                    query,
                    $"{prefix}.From",
                    dateRange.From);

                AddDateRangeInput(
                    query,
                    $"{prefix}.To",
                    dateRange.To);
            }

            if (filter.NumberRange is { IsEmpty: false } numberRange)
            {
                string prefix =
                    $"Filters[{name}].NumberRange";

                AddNumberRangeInput(
                    query,
                    prefix,
                    numberRange);
            }
        }
    }

    private static void AddDateRangeInput(
        QueryBuilder query,
        string prefix,
        DateInputViewModel date)
    {
        AddIfNotEmpty(
            query,
            $"{prefix}.Day",
            date.Day);

        AddIfNotEmpty(
            query,
            $"{prefix}.Month",
            date.Month);

        AddIfNotEmpty(
            query,
            $"{prefix}.Year",
            date.Year);
    }

    private static void AddNumberRangeInput(
        QueryBuilder query,
        string prefix,
        NumberRangeInputFilterViewModel range)
    {
        AddIfNotEmpty(
            query,
            $"{prefix}.From",
            range.From?.ToString() ?? string.Empty);

        AddIfNotEmpty(
            query,
            $"{prefix}.To",
            range.To?.ToString() ?? string.Empty);
    }

    private static void AddIfNotEmpty(
        QueryBuilder query,
        string key,
        string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            query.Add(
                key,
                value);
        }
    }
}
