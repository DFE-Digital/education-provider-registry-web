using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Exceptions;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Controllers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Controllers;

public sealed class FilterValidationErrorsTests
{
    [Fact]
    public void Apply_GivenNullModelState_ThrowsArgumentNullException()
    {
        // Arrange
        IReadOnlyCollection<FilterValidationError> validationErrors = [];

        // Act
        Action apply = () =>
            FilterValidationErrors.Apply(
                null!,
                validationErrors);

        // Assert
        Assert.Throws<ArgumentNullException>(apply);
    }

    [Fact]
    public void Apply_GivenNullValidationErrors_ThrowsArgumentNullException()
    {
        // Arrange
        ModelStateDictionary modelState = new();

        // Act
        Action apply = () =>
            FilterValidationErrors.Apply(
                modelState,
                null!);

        // Assert
        Assert.Throws<ArgumentNullException>(apply);
    }

    [Fact]
    public void Apply_GivenNoValidationErrors_DoesNotAddModelStateErrors()
    {
        // Arrange
        ModelStateDictionary modelState = new();

        // Act
        FilterValidationErrors.Apply(
            modelState,
            []);

        // Assert
        Assert.Empty(modelState);
    }

    [Theory]
    [InlineData(
        SearchFilterKeys.OpenedDate,
        ValidationErrorCode.LowerNotValid,
        SearchFilterModelStateKeys.OpenedDateFrom,
        "'Open from' date must be a real date")]
    [InlineData(
        SearchFilterKeys.OpenedDate,
        ValidationErrorCode.UpperNotValid,
        SearchFilterModelStateKeys.OpenedDateTo,
        "'Open to' date must be a real date")]
    [InlineData(
        SearchFilterKeys.OpenedDate,
        ValidationErrorCode.LowerBeforeMinimum,
        SearchFilterModelStateKeys.OpenedDateFrom,
        "'Open from' date must be after 1 January 1800")]
    [InlineData(
        SearchFilterKeys.OpenedDate,
        ValidationErrorCode.UpperBeforeMinimum,
        SearchFilterModelStateKeys.OpenedDateTo,
        "'Open to' date must be after 1 January 1800")]
    [InlineData(
        SearchFilterKeys.OpenedDate,
        ValidationErrorCode.UpperNotGreaterThanLower,
        SearchFilterModelStateKeys.OpenedDateTo,
        "'Open to' date must be after 'From' date")]

    [InlineData(
        SearchFilterKeys.ClosedDate,
        ValidationErrorCode.LowerNotValid,
        SearchFilterModelStateKeys.ClosedDateFrom,
        "'Closed from' date must be a real date")]
    [InlineData(
        SearchFilterKeys.ClosedDate,
        ValidationErrorCode.UpperNotValid,
        SearchFilterModelStateKeys.ClosedDateTo,
        "'Closed to' date must be a real date")]
    [InlineData(
        SearchFilterKeys.ClosedDate,
        ValidationErrorCode.LowerBeforeMinimum,
        SearchFilterModelStateKeys.ClosedDateFrom,
        "'Closed from' date must be after 1 January 1800")]
    [InlineData(
        SearchFilterKeys.ClosedDate,
        ValidationErrorCode.UpperBeforeMinimum,
        SearchFilterModelStateKeys.ClosedDateTo,
        "'Closed to' date must be after 1 January 1800")]
    [InlineData(
        SearchFilterKeys.ClosedDate,
        ValidationErrorCode.UpperNotGreaterThanLower,
        SearchFilterModelStateKeys.ClosedDateTo,
        "'Closed to' date must be after 'From' date")]

    [InlineData(
        SearchFilterKeys.AgeRange,
        ValidationErrorCode.LowerNotValid,
        SearchFilterModelStateKeys.AgeRangeFrom,
        "'From' age must be a number")]
    [InlineData(
        SearchFilterKeys.AgeRange,
        ValidationErrorCode.UpperNotValid,
        SearchFilterModelStateKeys.AgeRangeTo,
        "'To' age must be a number")]
    [InlineData(
        SearchFilterKeys.AgeRange,
        ValidationErrorCode.LowerBeforeMinimum,
        SearchFilterModelStateKeys.AgeRangeFrom,
        "'From' age must be 0 or more")]
    [InlineData(
        SearchFilterKeys.AgeRange,
        ValidationErrorCode.UpperBeforeMinimum,
        SearchFilterModelStateKeys.AgeRangeTo,
        "'To' age must be 0 or more")]
    [InlineData(
        SearchFilterKeys.AgeRange,
        ValidationErrorCode.LowerAfterMaximum,
        SearchFilterModelStateKeys.AgeRangeFrom,
        "'From' age must be 99 or less")]
    [InlineData(
        SearchFilterKeys.AgeRange,
        ValidationErrorCode.UpperAfterMaximum,
        SearchFilterModelStateKeys.AgeRangeTo,
        "'To' age must be 99 or less")]
    [InlineData(
        SearchFilterKeys.AgeRange,
        ValidationErrorCode.UpperNotGreaterThanLower,
        SearchFilterModelStateKeys.AgeRangeTo,
        "'To' age must be before 'From' age")]
    public void Apply_GivenValidationError_AddsExpectedModelStateError(
        string filterName,
        ValidationErrorCode errorCode,
        string expectedModelStateKey,
        string expectedMessage)
    {
        // Arrange
        ModelStateDictionary modelState = new();

        FilterValidationError validationError = new(
            filterName,
            errorCode);

        // Act
        FilterValidationErrors.Apply(
            modelState,
            [validationError]);

        // Assert
        ModelStateEntry entry =
            Assert.IsType<ModelStateEntry>(
                modelState[expectedModelStateKey], exactMatch: false);

        ModelError error =
            Assert.Single(entry.Errors);

        Assert.Equal(
            expectedMessage,
            error.ErrorMessage);
    }

    [Fact]
    public void Apply_GivenMultipleValidationErrors_AddsAllErrors()
    {
        // Arrange
        ModelStateDictionary modelState = new();

        IReadOnlyCollection<FilterValidationError> validationErrors =
        [
            new(
                SearchFilterKeys.OpenedDate,
                ValidationErrorCode.LowerNotValid),

            new(
                SearchFilterKeys.OpenedDate,
                ValidationErrorCode.UpperNotValid),

            new(
                SearchFilterKeys.AgeRange,
                ValidationErrorCode.LowerBeforeMinimum)
        ];

        // Act
        FilterValidationErrors.Apply(
            modelState,
            validationErrors);

        // Assert
        Assert.Equal(
            3,
            modelState.ErrorCount);

        Assert.Single(
            modelState[
                SearchFilterModelStateKeys.OpenedDateFrom]!
                .Errors);

        Assert.Single(
            modelState[
                SearchFilterModelStateKeys.OpenedDateTo]!
                .Errors);

        Assert.Single(
            modelState[
                SearchFilterModelStateKeys.AgeRangeFrom]!
                .Errors);
    }

    [Fact]
    public void Apply_GivenMultipleErrorsForSameField_AddsAllErrorsToField()
    {
        // Arrange
        ModelStateDictionary modelState = new();

        IReadOnlyCollection<FilterValidationError> validationErrors =
        [
            new(
                SearchFilterKeys.AgeRange,
                ValidationErrorCode.UpperNotValid),

            new(
                SearchFilterKeys.AgeRange,
                ValidationErrorCode.UpperAfterMaximum)
        ];

        // Act
        FilterValidationErrors.Apply(
            modelState,
            validationErrors);

        // Assert
        ModelStateEntry entry =
            Assert.IsType<ModelStateEntry>(
                modelState[
                    SearchFilterModelStateKeys.AgeRangeTo], exactMatch: false);

        Assert.Equal(
            2,
            entry.Errors.Count);

        Assert.Contains(
            entry.Errors,
            error =>
                error.ErrorMessage ==
                "'To' age must be a number");

        Assert.Contains(
            entry.Errors,
            error =>
                error.ErrorMessage ==
                "'To' age must be 99 or less");
    }

    [Fact]
    public void Apply_GivenUnsupportedFilterName_ThrowsInvalidOperationException()
    {
        // Arrange
        ModelStateDictionary modelState = new();

        FilterValidationError validationError = new(
            "unsupported_filter",
            ValidationErrorCode.LowerNotValid);

        // Act
        Action apply = () =>
            FilterValidationErrors.Apply(
                modelState,
                [validationError]);

        // Assert
        Assert.Throws<InvalidOperationException>(apply);
    }
}
