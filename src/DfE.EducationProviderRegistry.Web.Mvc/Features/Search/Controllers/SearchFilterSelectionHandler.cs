using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Services;

public sealed class SearchFilterSelectionHandler :
    ISearchFilterSelectionHandler
{
    public void Handle(SearchRequestViewModel request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.ClearFilters)
        {
            ClearFilters(request);
            return;
        }

        RemoveSelectedFilter(request);
    }

    private static void ClearFilters(
        SearchRequestViewModel request)
    {
        request.SelectedFacets.Clear();
        request.ClearFilters = false;
        request.RemoveFilter = null;
    }

    private static void RemoveSelectedFilter(
        SearchRequestViewModel request)
    {
        if (!TryParseRemoveFilter(
                request.RemoveFilter,
                out string facetName,
                out string value))
        {
            return;
        }

        request.RemoveFilter = null;

        if (!request.SelectedFacets.TryGetValue(
                facetName,
                out List<string>? selectedValues))
        {
            return;
        }

        selectedValues.RemoveAll(selectedValue =>
            string.Equals(
                selectedValue,
                value,
                StringComparison.OrdinalIgnoreCase));

        if (selectedValues.Count == 0)
        {
            request.SelectedFacets.Remove(facetName);
        }
    }

    private static bool TryParseRemoveFilter(
        string? removeFilter,
        out string facetName,
        out string value)
    {
        facetName = string.Empty;
        value = string.Empty;

        if (string.IsNullOrWhiteSpace(removeFilter))
        {
            return false;
        }

        string[] parts = removeFilter.Split(
            '|',
            count: 2,
            StringSplitOptions.TrimEntries);

        if (parts.Length != 2 ||
            string.IsNullOrWhiteSpace(parts[0]) ||
            string.IsNullOrWhiteSpace(parts[1]))
        {
            return false;
        }

        facetName = parts[0];
        value = parts[1];

        return true;
    }
}