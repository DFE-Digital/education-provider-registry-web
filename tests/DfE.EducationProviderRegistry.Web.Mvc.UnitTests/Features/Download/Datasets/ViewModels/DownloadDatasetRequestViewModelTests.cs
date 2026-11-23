using System.ComponentModel.DataAnnotations;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Datasets.ViewModels;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Download.Datasets.ViewModels;

public sealed class DownloadDatasetRequestViewModelTests
{
    private static IList<ValidationResult> Validate(object model)
    {
        ValidationContext context = new(model);
        IList<ValidationResult> results = [];
        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }

    [Fact]
    public void SelectedDatasets_IsInvalid_WhenEmpty()
    {
        // arrange
        DownloadDatasetRequestViewModel viewModel =
            new()
            {
                SelectedDatasets = []
            };

        // act
        IList<ValidationResult> results = Validate(viewModel);

        // assert
        Assert.Single(results);
        Assert.Equal("Choose at least one dataset file download name.", results[0].ErrorMessage);
        Assert.Contains("SelectedDatasets", results[0].MemberNames);
    }

    [Fact]
    public void SelectedDatasets_ValidatesSuccessfully_WhenItemsProvided()
    {
        // arrange
        DownloadDatasetRequestViewModel viewModel =
            new()
            {
                SelectedDatasets = ["establishment-data"]
            };

        // act
        IList<ValidationResult> results = Validate(viewModel);

        // assert
        Assert.Empty(results);
    }
}
