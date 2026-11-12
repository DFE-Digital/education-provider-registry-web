namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.Filters;

public static class FilterInputId
{
    public static string? ForModelStateKey(string modelStateKey)
    {
        return modelStateKey switch
        {
            SearchFilterModelStateKeys.OpenedDateFrom =>
                "filter-opened-date-from-day",

            SearchFilterModelStateKeys.OpenedDateTo =>
                "filter-opened-date-to-day",

            SearchFilterModelStateKeys.ClosedDateFrom =>
                "filter-closed-date-from-day",

            SearchFilterModelStateKeys.ClosedDateTo =>
                "filter-closed-date-to-day",

            SearchFilterModelStateKeys.AgeRangeFrom =>
                "filter-age-range-from",

            SearchFilterModelStateKeys.AgeRangeTo =>
                "filter-age-range-to",

            _ => null
        };
    }
}
