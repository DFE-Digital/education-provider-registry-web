using DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Datasets.ViewModels;
using System.ComponentModel.DataAnnotations;

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
    public void Filename_IsRequired_WhenNull()
    {
        // arrange
        DownloadDatasetRequestViewModel viewModel =
            new(){
                Filename = null
            };

        // act
        IList<ValidationResult> results = Validate(viewModel);

        // assert
        Assert.Single(results);
        Assert.Equal("A download file name is required.", results[0].ErrorMessage);
        Assert.Contains("Filename", results[0].MemberNames);
    }

    [Fact]
    public void Filename_IsRequired_WhenEmptyString()
    {
        // arrange
        DownloadDatasetRequestViewModel viewModel =
            new(){
                Filename = string.Empty
            };

        // act
        IList<ValidationResult> results = Validate(viewModel);

        // arrange
        Assert.Single(results);
        Assert.Equal("A download file name is required.", results[0].ErrorMessage);
        Assert.Contains("Filename", results[0].MemberNames);
    }

    [Fact]
    public void Filename_ValidatesSuccessfully_WhenProvided()
    {
        // arrange
        DownloadDatasetRequestViewModel viewModel =
            new(){
                Filename = "dataset.csv"
            };

        // act
        IList<ValidationResult> results = Validate(viewModel);

        // assert
        Assert.Empty(results);
    }
}
