using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Exceptions;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Controllers;

public static class SearchFilterKeys
{
    public const string OpenedDate = "opened_date";
    public const string ClosedDate = "closed_date";
    public const string AgeRange = "age_range";
}

public static class FilterValidationErrors
{
    private sealed record FilterModelStateError(string ModelStateKey, string Message);

    public static void Apply(
        ModelStateDictionary modelState,
        IReadOnlyCollection<FilterValidationError> validationErrors)
    {
        ArgumentNullException.ThrowIfNull(modelState);
        ArgumentNullException.ThrowIfNull(validationErrors);

        foreach (FilterValidationError validationError in validationErrors)
        {
            FilterModelStateError error =
                Map(validationError);

            modelState.AddModelError(
                error.ModelStateKey,
                error.Message);
        }
    }

    private static FilterModelStateError Map(
        FilterValidationError error)
    {
        return (error.FilterName, error.Error) switch
        {
            // Opened date
            (
                SearchFilterKeys.OpenedDate,
                ValidationErrorCode.LowerNotValid
            ) => new(
                SearchFilterModelStateKeys.OpenedDateFrom,
                "'Open from' date must be a real date"),

            (
                SearchFilterKeys.OpenedDate,
                ValidationErrorCode.UpperNotValid
            ) => new(
                SearchFilterModelStateKeys.OpenedDateTo,
                "'Open to' date must be a real date"),

            (
                SearchFilterKeys.OpenedDate,
                ValidationErrorCode.LowerBeforeMinimum
            ) => new(
                SearchFilterModelStateKeys.OpenedDateFrom,
                "'Open from' date must be after 1 January 1800"),

            (
                SearchFilterKeys.OpenedDate,
                ValidationErrorCode.UpperBeforeMinimum
            ) => new(
                SearchFilterModelStateKeys.OpenedDateTo,
                "'Open to' date must be after 1 January 1800"),

            (
                SearchFilterKeys.OpenedDate,
                ValidationErrorCode.UpperNotGreaterThanLower
            ) => new(
                SearchFilterModelStateKeys.OpenedDateTo,
                "'Open to' date must be after 'From' date"),

            // Closed date
            (
                SearchFilterKeys.ClosedDate,
                ValidationErrorCode.LowerNotValid
            ) => new(
                SearchFilterModelStateKeys.ClosedDateFrom,
                "'Closed from' date must be a real date"),

            (
                SearchFilterKeys.ClosedDate,
                ValidationErrorCode.UpperNotValid
            ) => new(
                SearchFilterModelStateKeys.ClosedDateTo,
                "'Closed to' date must be a real date"),

            (
                SearchFilterKeys.ClosedDate,
                ValidationErrorCode.LowerBeforeMinimum
            ) => new(
                SearchFilterModelStateKeys.ClosedDateFrom,
                "'Closed from' date must be after 1 January 1800"),

            (
                SearchFilterKeys.ClosedDate,
                ValidationErrorCode.UpperBeforeMinimum
            ) => new(
                SearchFilterModelStateKeys.ClosedDateTo,
                "'Closed to' date must be after 1 January 1800"),

            (
                SearchFilterKeys.ClosedDate,
                ValidationErrorCode.UpperNotGreaterThanLower
            ) => new(
                SearchFilterModelStateKeys.ClosedDateTo,
                "'Closed to' date must be after 'From' date"),

            // Age range
            (
                SearchFilterKeys.AgeRange,
                ValidationErrorCode.LowerNotValid
            ) => new(
                SearchFilterModelStateKeys.AgeRangeFrom,
                "'From' age must be a number"),

            (
                SearchFilterKeys.AgeRange,
                ValidationErrorCode.UpperNotValid
            ) => new(
                SearchFilterModelStateKeys.AgeRangeTo,
                "'To' age must be a number"),

            (
                SearchFilterKeys.AgeRange,
                ValidationErrorCode.LowerBeforeMinimum
            ) => new(
                SearchFilterModelStateKeys.AgeRangeFrom,
                "'From' age must be 0 or more"),

            (
                SearchFilterKeys.AgeRange,
                ValidationErrorCode.UpperBeforeMinimum
            ) => new(
                SearchFilterModelStateKeys.AgeRangeTo,
                "'To' age must be 0 or more"),

            (
                SearchFilterKeys.AgeRange,
                ValidationErrorCode.LowerAfterMaximum
            ) => new(
                SearchFilterModelStateKeys.AgeRangeFrom,
                "'From' age must be 99 or less"),

            (
                SearchFilterKeys.AgeRange,
                ValidationErrorCode.UpperAfterMaximum
            ) => new(
                SearchFilterModelStateKeys.AgeRangeTo,
                "'To' age must be 99 or less"),

            (
                SearchFilterKeys.AgeRange,
                ValidationErrorCode.UpperNotGreaterThanLower
            ) => new(
                SearchFilterModelStateKeys.AgeRangeTo,
                "'To' age must be before 'From' age"),

            _ => throw new InvalidOperationException($"No Web validation mapping exists for filter '{error.FilterName}' and error '{error.Error}.")
        };
    }
}
