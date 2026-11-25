using System.ComponentModel.DataAnnotations;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Datasets.ViewModels.Validation;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Download.Datasets.ViewModels.Validation;

public sealed class NonEmptyListAttributeTests
{
    [Fact]
    public void IsValid_ReturnsFalse_WhenValueIsNull()
    {
        // arrange
        NonEmptyListAttribute attribute = new();

        // act
        bool result = attribute.IsValid(null);

        // assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_ReturnsFalse_WhenValueIsString()
    {
        // arrange
        NonEmptyListAttribute attribute = new();

        // act
        bool result = attribute.IsValid("non-empty-string");

        // assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_ReturnsFalse_WhenValueIsNotEnumerable()
    {
        // arrange
        NonEmptyListAttribute attribute = new();

        // act
        bool result = attribute.IsValid(42);

        // assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_ReturnsFalse_WhenCollectionIsEmpty()
    {
        // arrange
        NonEmptyListAttribute attribute = new();
        List<string> emptyList = [];

        // act
        bool result = attribute.IsValid(emptyList);

        // assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_ReturnsTrue_WhenCollectionHasElements()
    {
        // arrange
        NonEmptyListAttribute attribute = new();
        List<string> populatedList = ["dataset-1"];

        // act
        bool result = attribute.IsValid(populatedList);

        // assert
        Assert.True(result);
    }

    [Fact]
    public void IsValid_ReturnsFalse_WhenNonCollectionEnumerableIsEmpty()
    {
        // arrange
        NonEmptyListAttribute attribute = new();
        IEnumerable<string> emptySequence = YieldItems(0);

        // act
        bool result = attribute.IsValid(emptySequence);

        // assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_ReturnsTrue_WhenNonCollectionEnumerableHasElements()
    {
        // arrange
        NonEmptyListAttribute attribute = new();
        IEnumerable<string> populatedSequence = YieldItems(2);

        // act
        bool result = attribute.IsValid(populatedSequence);

        // assert
        Assert.True(result);
    }

    [Fact]
    public void TryValidateObject_FailsWithDefaultMessage_WhenListIsEmpty()
    {
        // arrange
        DefaultMessageModel model = new()
        {
            Items = []
        };

        ValidationContext context = new(model);
        List<ValidationResult> results = [];

        // act
        bool isValid =
            Validator.TryValidateObject(
                model, context, results, validateAllProperties: true);

        // assert
        Assert.False(isValid);
        ValidationResult failure = Assert.Single(results);
        Assert.Equal("Select at least one dataset.", failure.ErrorMessage);
        Assert.Contains(nameof(DefaultMessageModel.Items), failure.MemberNames);
    }

    [Fact]
    public void TryValidateObject_FailsWithCustomMessage_WhenCustomErrorMessageProvided()
    {
        // arrange
        CustomMessageModel model = new()
        {
            Items = []
        };

        ValidationContext context = new(model);
        List<ValidationResult> results = [];

        // act
        bool isValid =
            Validator.TryValidateObject(
                model, context, results, validateAllProperties: true);

        // assert
        Assert.False(isValid);
        ValidationResult failure = Assert.Single(results);
        Assert.Equal("Choose at least one option.", failure.ErrorMessage);
        Assert.Contains(nameof(CustomMessageModel.Items), failure.MemberNames);
    }

    [Fact]
    public void TryValidateObject_Succeeds_WhenSequenceHasElements()
    {
        // arrange
        SequenceModel model = new()
        {
            Items = YieldItems(1)
        };

        ValidationContext context = new(model);
        List<ValidationResult> results = [];

        // act
        bool isValid =
            Validator.TryValidateObject(
                model, context, results, validateAllProperties: true);

        // assert
        Assert.True(isValid);
        Assert.Empty(results);
    }

    private sealed class DefaultMessageModel
    {
        [NonEmptyList]
        public List<string>? Items { get; set; } = [];
    }

    private sealed class CustomMessageModel
    {
        [NonEmptyList("Choose at least one option.")]
        public List<string>? Items { get; set; } = [];
    }

    private sealed class SequenceModel
    {
        [NonEmptyList]
        public IEnumerable<string> Items { get; set; } = [];
    }

    private static IEnumerable<string> YieldItems(int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return $"item-{i}";
        }
    }
}
