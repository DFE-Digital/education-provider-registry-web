namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

public static class SearchFilterModelStateKeys
{
    public const string OpenedDateFrom = "Filters[opened_date].DateRange.From";
    public const string OpenedDateTo = "Filters[opened_date].DateRange.To";
    public const string ClosedDateFrom = "Filters[closed_date].DateRange.From";
    public const string ClosedDateTo = "Filters[closed_date].DateRange.To";
    public const string AgeRangeFrom = "Filters[age_range].NumberRange.From";
    public const string AgeRangeTo = "Filters[age_range].NumberRange.To";
}
