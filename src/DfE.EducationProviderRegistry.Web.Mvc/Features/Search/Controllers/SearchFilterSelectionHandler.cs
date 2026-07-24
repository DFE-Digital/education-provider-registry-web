using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Services;

public class SearchFilterSelectionHandler :
    ISearchFilterSelectionHandler
{
    private const string SelectedFacetsPrefix = "SelectedFacets[";
    private const string SelectedFacetsSuffix = "]";

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
                out string? facetName,
                out string? value))
        {
            return;
        }

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

        request.RemoveFilter = null;
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

        string bindingName = parts[0];

        if (!bindingName.StartsWith(
                SelectedFacetsPrefix,
                StringComparison.Ordinal) ||
            !bindingName.EndsWith(
                SelectedFacetsSuffix,
                StringComparison.Ordinal))
        {
            return false;
        }

        int facetNameLength =
            bindingName.Length -
            SelectedFacetsPrefix.Length -
            SelectedFacetsSuffix.Length;

        if (facetNameLength <= 0)
        {
            return false;
        }

        facetName = bindingName.Substring(
            SelectedFacetsPrefix.Length,
            facetNameLength);

        value = parts[1];

        return true;
    }
}