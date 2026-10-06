using System.Reflection.Metadata.Ecma335;
using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.Core.Libraries.Testing.Logger;
using DfE.Core.Libraries.Testing.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Groups;
using DfE.EducationProviderRegistry.Web.Mvc.UnitTests.TestDoubles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Groups;

public sealed class GroupsControllerTests
{
    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        // Arrange
        Func<GroupsController> construct =
            () => new(
                logger: null!,
                useCase: IUseCaseTestDoubles.Default<GetGroupByGroupUniqueIdentifierRequest, UseCaseResponse<GroupReadModelResponse>>(),
                mapper: IMapperTestDouble.Default<GroupReadModel, GroupDetailsPageViewModel>());

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenUseCaseIsNull()
    {
        // Arrange
        Func<GroupsController> construct =
            () => new(
                logger: ILoggerTestDouble.Default<GroupsController>(),
                useCase: null!,
                mapper: IMapperTestDouble.Default<GroupReadModel, GroupDetailsPageViewModel>());

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenMapperIsNull()
    {
        // Arrange
        Func<GroupsController> construct =
            () => new(
                logger: ILoggerTestDouble.Default<GroupsController>(),
                useCase: IUseCaseTestDoubles.Default<GetGroupByGroupUniqueIdentifierRequest, UseCaseResponse<GroupReadModelResponse>>(),
                mapper: null!);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public async Task Details_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        GroupsController controller = CreateSut();

        controller.ModelState.AddModelError("key", "error");

        string groupId = "group-1";

        // Act
        IActionResult result = await controller.Details(groupId);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Details_ReturnsStatusCode500_WhenUseCaseFails()
    {
        // Arrange

        GroupReadModelResponse response = GroupReadModelResponseTestDoubles.Failure();

        GroupsController controller =
            CreateSut(
                useCase:
                    IUseCaseTestDoubles.WithResponse<GetGroupByGroupUniqueIdentifierRequest, GroupReadModelResponse>(
                        response: UseCaseResponse<GroupReadModelResponse>.Failure(response, "error")));

        string groupId = "group-1";

        // Act
        IActionResult result = await controller.Details(groupId);

        // Assert
        StatusCodeResult objectResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public async Task Details_ReturnsNotFound_WhenNoModelReturned()
    {
        // Arrange
        GroupsController controller =
            CreateSut(
                useCase: IUseCaseTestDoubles.WithResponse<GetGroupByGroupUniqueIdentifierRequest, GroupReadModelResponse>(
                    response: UseCaseResponse<GroupReadModelResponse>.Success(
                        GroupReadModelResponseTestDoubles.Failure())));

        string groupId = "group-1";

        // Act
        IActionResult result = await controller.Details(groupId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_ReturnsView_WithMappedModel_WhenSuccessful()
    {
        // Arrange
        GroupDetailsPageViewModel viewModel = new()
        {
            Academies = new(),
            Details = new(),
            Governance = new(),
            Heading = "heading"
        };

        Mock<IMapper<GroupReadModel, GroupDetailsPageViewModel>> mapperMock =
            IMapperTestDouble.Map<GroupReadModel, GroupDetailsPageViewModel>(viewModel);

        GroupReadModelResponse response = GroupReadModelResponseTestDoubles.Stub();

        UseCaseResponse<GroupReadModelResponse> useCaseResponse = UseCaseResponse<GroupReadModelResponse>.Success(response);

        GroupsController controller =
            CreateSut(
                useCase: IUseCaseTestDoubles.WithResponse<GetGroupByGroupUniqueIdentifierRequest, GroupReadModelResponse>(useCaseResponse),
                mapper: mapperMock.Object);

        string groupId = "group-1";

        // Act
        IActionResult result = await controller.Details(groupId);

        // Assert
        ViewResult viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(viewModel, viewResult.Model);
    }


    private static GroupsController CreateSut(
        ILogger<GroupsController>? logger = null,
        IUseCase<GetGroupByGroupUniqueIdentifierRequest, UseCaseResponse<GroupReadModelResponse>>? useCase = null,
        IMapper<GroupReadModel, GroupDetailsPageViewModel>? mapper = null)
    {
        return new GroupsController(
            logger ?? ILoggerTestDouble.Default<GroupsController>(),
            useCase ?? IUseCaseTestDoubles.Default<GetGroupByGroupUniqueIdentifierRequest, UseCaseResponse<GroupReadModelResponse>>(),
            mapper ?? IMapperTestDouble.Default<GroupReadModel, GroupDetailsPageViewModel>());
    }
}

public static class GroupReadModelResponseTestDoubles
{
    public static GroupReadModelResponse Failure()
    {
        return new()
        {
            Group = null
        };
    }

    public static GroupReadModelResponse Stub()
    {
        return new GroupReadModelResponse()
        {
            Group = GroupReadModelTestDoubles.Stub()
        };
    }
}
