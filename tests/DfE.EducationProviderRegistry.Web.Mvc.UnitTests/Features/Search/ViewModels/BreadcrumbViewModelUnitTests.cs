using DfE.EducationProviderRegistry.Web.Mvc.ViewModels;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.ViewModels;

public sealed class BreadcrumbViewModelUnitTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        // arrange
        string text = "test";
        string controller = "controller";
        string action = "action";

        // act
        BreadcrumbViewModel vm = new(text, controller, action);

        // assert
        Assert.Equal(text, vm.Text);
        Assert.Equal(controller, vm.Controller);
        Assert.Equal(action, vm.Action);
    }

    [Fact]
    public void Constructor_AllowsNullCount()
    {
        // arrange
        string text = "test";

        // act
        BreadcrumbViewModel vm = new(text);

        // assert
        Assert.Null(vm.Action);
        Assert.Null(vm.Controller);
        Assert.Equal("test", vm.Text);
    }
}
