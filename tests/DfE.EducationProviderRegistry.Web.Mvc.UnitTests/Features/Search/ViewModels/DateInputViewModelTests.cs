using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.Request;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.ViewModels;

public sealed class DateInputViewModelTests
{
    [Fact]
    public void IsEmpty_ReturnsTrue_WhenAllValuesAreNull()
    {
        // arrange
        DateInputViewModel sut = new();

        // act/assert
        Assert.True(sut.IsEmpty);
    }

    [Fact]
    public void IsEmpty_ReturnsTrue_WhenAllValuesAreWhitespace()
    {
        // arrange
        DateInputViewModel sut = new()
        {
            Day = " ",
            Month = "\t",
            Year = " "
        };

        // act/assert
        Assert.True(sut.IsEmpty);
    }

    [Fact]
    public void IsEmpty_ReturnsFalse_WhenDayHasValue()
    {
        // arrange
        DateInputViewModel sut = new()
        {
            Day = "1"
        };

        // act/assert
        Assert.False(sut.IsEmpty);
    }

    [Fact]
    public void IsEmpty_ReturnsFalse_WhenMonthHasValue()
    {
        // arrange
        DateInputViewModel sut = new()
        {
            Month = "1"
        };

        // act/assert
        Assert.False(sut.IsEmpty);
    }

    [Fact]
    public void IsEmpty_ReturnsFalse_WhenYearHasValue()
    {
        // arrange
        DateInputViewModel sut = new()
        {
            Year = "2024"
        };

        // act/assert
        Assert.False(sut.IsEmpty);
    }

    [Fact]
    public void TryGetDate_ReturnsTrue_ForValidDate()
    {
        // arrange
        DateInputViewModel sut = new()
        {
            Day = "1",
            Month = "1",
            Year = "2024"
        };

        // act
        bool result = sut.TryGetDate(out DateOnly date);

        // assert
        Assert.True(result);
        Assert.Equal(new DateOnly(2024, 1, 1), date);
    }

    [Fact]
    public void TryGetDate_ReturnsTrue_WhenValuesContainWhitespace()
    {
        // arrange
        DateInputViewModel sut = new()
        {
            Day = " 1 ",
            Month = " 1 ",
            Year = " 2024 "
        };

        // act
        bool result = sut.TryGetDate(out DateOnly date);

        // assert
        Assert.True(result);
        Assert.Equal(new DateOnly(2024, 1, 1), date);
    }

    [Fact]
    public void TryGetDate_ReturnsFalse_WhenYearIsNull()
    {
        // arrange
        DateInputViewModel sut = new()
        {
            Day = "1",
            Month = "1"
        };

        // act
        bool result = sut.TryGetDate(out DateOnly date);

        // assert
        Assert.False(result);
        Assert.Equal(default, date);
    }

    [Fact]
    public void TryGetDate_ReturnsFalse_WhenYearIsNotFourDigits()
    {
        // arrange
        DateInputViewModel sut = new()
        {
            Day = "1",
            Month = "1",
            Year = "24"
        };

        // act
        bool result = sut.TryGetDate(out DateOnly date);

        // assert
        Assert.False(result);
        Assert.Equal(default, date);
    }

    [Fact]
    public void TryGetDate_ReturnsFalse_WhenYearContainsNonNumericCharacters()
    {
        // arrange
        DateInputViewModel sut = new()
        {
            Day = "1",
            Month = "1",
            Year = "ABCD"
        };

        // act
        bool result = sut.TryGetDate(out DateOnly date);

        // assert
        Assert.False(result);
        Assert.Equal(default, date);
    }

    [Fact]
    public void TryGetDate_ReturnsFalse_WhenMonthIsNotNumeric()
    {
        // arrange
        DateInputViewModel sut = new()
        {
            Day = "1",
            Month = "AA",
            Year = "2024"
        };

        // act
        bool result = sut.TryGetDate(out DateOnly date);

        // assert
        Assert.False(result);
        Assert.Equal(default, date);
    }

    [Fact]
    public void TryGetDate_ReturnsFalse_WhenDayIsNotNumeric()
    {
        // arrange
        DateInputViewModel sut = new()
        {
            Day = "AA",
            Month = "1",
            Year = "2024"
        };

        // act
        bool result = sut.TryGetDate(out DateOnly date);

        // assert
        Assert.False(result);
        Assert.Equal(default, date);
    }

    [Theory]
    [InlineData("0000")]
    [InlineData("10000")]
    public void TryGetDate_ReturnsFalse_WhenYearIsOutOfRange(
        string year)
    {
        // arrange
        DateInputViewModel sut = new()
        {
            Day = "1",
            Month = "1",
            Year = year
        };

        // act
        bool result = sut.TryGetDate(out _);

        // assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("13")]
    public void TryGetDate_ReturnsFalse_WhenMonthIsOutOfRange(
        string month)
    {
        // arrange
        DateInputViewModel sut = new()
        {
            Day = "1",
            Month = month,
            Year = "2024"
        };

        // act
        bool result = sut.TryGetDate(out _);

        // assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("32")]
    public void TryGetDate_ReturnsFalse_WhenDayIsOutOfRange(
        string day)
    {
        // arrange
        DateInputViewModel sut = new()
        {
            Day = day,
            Month = "1",
            Year = "2024"
        };

        // act
        bool result = sut.TryGetDate(out _);

        // assert
        Assert.False(result);
    }

    [Fact]
    public void TryGetDate_ReturnsFalse_WhenDayExceedsDaysInMonth()
    {
        // arrange
        DateInputViewModel sut = new()
        {
            Day = "31",
            Month = "4",
            Year = "2024"
        };

        // act
        bool result = sut.TryGetDate(out _);

        // assert
        Assert.False(result);
    }

    [Fact]
    public void TryGetDate_ReturnsFalse_ForInvalidLeapYearDate()
    {
        // arrange
        DateInputViewModel sut = new()
        {
            Day = "29",
            Month = "2",
            Year = "2023"
        };

        // act
        bool result = sut.TryGetDate(out _);

        // assert
        Assert.False(result);
    }

    [Fact]
    public void TryGetDate_ReturnsTrue_ForValidLeapYearDate()
    {
        // arrange
        DateInputViewModel sut = new()
        {
            Day = "29",
            Month = "2",
            Year = "2024"
        };

        // act
        bool result = sut.TryGetDate(out DateOnly date);

        // assert
        Assert.True(result);
        Assert.Equal(new DateOnly(2024, 2, 29), date);
    }
}
