using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering.Options;
using System.ComponentModel.DataAnnotations;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Search.Infrastructure.Core.Filtering.Options;

public sealed class FilterKeyToFilterExpressionMapOptionsUnitTests
{
    [Fact]
    public void FilterChainingLogicalOperator_WhenNull_FailsValidation()
    {
        FilterKeyToFilterExpressionMapOptions options =
            new FilterKeyToFilterExpressionMapOptions
            {
                FilterChainingLogicalOperator = null,
                SearchFilterToExpressionMap = new Dictionary<string, FilterExpressionOptions>
                {
                    { "Key1", new FilterExpressionOptions() }
                }
            };

        ValidationContext context = new ValidationContext(options);
        List<ValidationResult> results = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(options, context, results, true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(options.FilterChainingLogicalOperator)));
    }

    [Fact]
    public void SearchFilterToExpressionMap_WhenEmpty_FailsValidation()
    {
        FilterKeyToFilterExpressionMapOptions options =
            new FilterKeyToFilterExpressionMapOptions
            {
                FilterChainingLogicalOperator = "AndLogicalOperator",
                SearchFilterToExpressionMap = new Dictionary<string, FilterExpressionOptions>()
            };

        ValidationContext context = new ValidationContext(options);
        List<ValidationResult> results = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(options, context, results, true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(options.SearchFilterToExpressionMap)));
    }

    [Fact]
    public void SearchFilterToExpressionMap_WithOneEntry_PassesValidation()
    {
        FilterKeyToFilterExpressionMapOptions options =
            new FilterKeyToFilterExpressionMapOptions
            {
                FilterChainingLogicalOperator = "AndLogicalOperator",
                SearchFilterToExpressionMap = new Dictionary<string, FilterExpressionOptions>
                {
                    { "Key1", new FilterExpressionOptions() }
                }
            };

        ValidationContext context = new ValidationContext(options);
        List<ValidationResult> results = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(options, context, results, true);

        Assert.True(isValid);
    }
}



