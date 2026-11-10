using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.ViewModels;

public sealed class NumberRangeFilterViewModelTests
{
    [Fact]
    public void IsEmpty_ReturnsTrue_WhenFromAndToAreNull()
    {
        // arrange
        NumberRangeFilterViewModel sut = new();

        // act/assert
        Assert.True(sut.IsEmpty);
    }

    [Fact]
    public void IsEmpty_ReturnsFalse_WhenFromHasValue()
    {
        // arrange
        NumberRangeFilterViewModel sut = new()
        {
            From = 10
        };

        // act/assert
        Assert.False(sut.IsEmpty);
    }

    [Fact]
    public void IsEmpty_ReturnsFalse_WhenToHasValue()
    {
        // arrange
        NumberRangeFilterViewModel sut = new()
        {
            To = 20
        };

        // act/assert
        Assert.False(sut.IsEmpty);
    }

    [Fact]
    public void SelectedLabel_ReturnsNull_WhenFromAndToAreNull()
    {
        // arrange
        NumberRangeFilterViewModel sut = new();

        // act
        string? result = sut.SelectedLabel;

        // assert
        Assert.Null(result);
    }

    [Fact]
    public void SelectedLabel_ReturnsRange_WhenFromAndToHaveValues()
    {
        // arrange
        NumberRangeFilterViewModel sut = new()
        {
            From = 10,
            To = 20
        };

        // act
        string? result = sut.SelectedLabel;

        // assert
        Assert.Equal(
            "10 to 20",
            result);
    }

    [Fact]
    public void SelectedLabel_ReturnsAndOver_WhenOnlyFromHasValue()
    {
        // arrange
        NumberRangeFilterViewModel sut = new()
        {
            From = 10
        };

        // act
        string? result = sut.SelectedLabel;

        // assert
        Assert.Equal(
            "10 and over",
            result);
    }

    [Fact]
    public void SelectedLabel_ReturnsUpTo_WhenOnlyToHasValue()
    {
        // arrange
        NumberRangeFilterViewModel sut = new()
        {
            To = 20
        };

        // act
        string? result = sut.SelectedLabel;

        // assert
        Assert.Equal(
            "up to 20",
            result);
    }

    [Fact]
    public void SelectedLabel_ReturnsRange_WhenFromAndToAreEqual()
    {
        // arrange
        NumberRangeFilterViewModel sut = new()
        {
            From = 10,
            To = 10
        };

        // act
        string? result = sut.SelectedLabel;

        // assert
        Assert.Equal(
            "10 to 10",
            result);
    }
}
