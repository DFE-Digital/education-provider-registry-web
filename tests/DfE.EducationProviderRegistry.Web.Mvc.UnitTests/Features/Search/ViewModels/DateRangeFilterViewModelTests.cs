using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.Request;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.ViewModels;

public sealed class DateRangeFilterViewModelTests
{
    [Fact]
    public void IsEmpty_ReturnsTrue_WhenFromAndToAreEmpty()
    {
        // arrange
        DateRangeInputFilterViewModel sut = new();

        // act/assert
        Assert.True(sut.IsEmpty);
    }

    [Fact]
    public void IsEmpty_ReturnsFalse_WhenFromHasValue()
    {
        // arrange
        DateRangeInputFilterViewModel sut = new()
        {
            From = CreateDateInput(1, 1, 2024)
        };

        // act/assert
        Assert.False(sut.IsEmpty);
    }

    [Fact]
    public void IsEmpty_ReturnsFalse_WhenToHasValue()
    {
        // arrange
        DateRangeInputFilterViewModel sut = new()
        {
            To = CreateDateInput(31, 12, 2024)
        };

        // act/assert
        Assert.False(sut.IsEmpty);
    }

    [Fact]
    public void SelectedLabel_ReturnsNull_WhenRangeIsEmpty()
    {
        // arrange
        DateRangeInputFilterViewModel sut = new();

        // act
        string? result = sut.SelectedLabel;

        // assert
        Assert.Null(result);
    }

    [Fact]
    public void SelectedLabel_ReturnsDateRange_WhenFromAndToAreValid()
    {
        // arrange
        DateRangeInputFilterViewModel sut = new()
        {
            From = CreateDateInput(1, 1, 2024),
            To = CreateDateInput(31, 12, 2024)
        };

        // act
        string? result = sut.SelectedLabel;

        // assert
        Assert.Equal(
            "01/01/2024 to 31/12/2024",
            result);
    }

    [Fact]
    public void SelectedLabel_ReturnsFromLabel_WhenOnlyFromIsValid()
    {
        // arrange
        DateRangeInputFilterViewModel sut = new()
        {
            From = CreateDateInput(1, 1, 2024)
        };

        // act
        string? result = sut.SelectedLabel;

        // assert
        Assert.Equal(
            "From 01/01/2024",
            result);
    }

    [Fact]
    public void SelectedLabel_ReturnsToLabel_WhenOnlyToIsValid()
    {
        // arrange
        DateRangeInputFilterViewModel sut = new()
        {
            To = CreateDateInput(31, 12, 2024)
        };

        // act
        string? result = sut.SelectedLabel;

        // assert
        Assert.Equal(
            "To 31/12/2024",
            result);
    }

    [Fact]
    public void SelectedLabel_ReturnsNull_WhenFromDateIsInvalid()
    {
        // arrange
        DateRangeInputFilterViewModel sut = new()
        {
            From = new DateInputViewModel
            {
                Day = "32",
                Month = "1",
                Year = "2024"
            }
        };

        // act
        string? result = sut.SelectedLabel;

        // assert
        Assert.Null(result);
    }

    [Fact]
    public void SelectedLabel_ReturnsNull_WhenToDateIsInvalid()
    {
        // arrange
        DateRangeInputFilterViewModel sut = new()
        {
            To = new DateInputViewModel
            {
                Day = "1",
                Month = "13",
                Year = "2024"
            }
        };

        // act
        string? result = sut.SelectedLabel;

        // assert
        Assert.Null(result);
    }

    private static DateInputViewModel CreateDateInput(
        int day,
        int month,
        int year) =>
        new()
        {
            Day = day.ToString(),
            Month = month.ToString(),
            Year = year.ToString()
        };
}
