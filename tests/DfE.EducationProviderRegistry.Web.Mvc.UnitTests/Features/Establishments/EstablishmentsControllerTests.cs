using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishmentByUrn;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Establishments;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments.ViewModels;
using DfE.EducationProviderRegistry.Web.SharedTests.TestDoubles.Establishments;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Establishments;

public sealed class EstablishmentsControllerTests
{
    [Fact]
    public void Constructor_WithNullMapper_ThrowsArgumentNullException()
    {
        // Arrange
        IUseCase<GetEstablishmentByUrnRequest, UseCaseResponse<EstablishmentDetailsReadModel>> useCase =
            Mock.Of<IUseCase<GetEstablishmentByUrnRequest, UseCaseResponse<EstablishmentDetailsReadModel>>>();

        Func<EstablishmentsController> construct = () =>
            new(null!, useCase);

        // Act / Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_WithNullUseCase_ThrowsArgumentNullException()
    {
        // Arrange
        IMapper<EstablishmentDetailsModel, EstablishmentDetailsPageViewModel> mapper =
            Mock.Of<IMapper<EstablishmentDetailsModel, EstablishmentDetailsPageViewModel>>();

        Func<EstablishmentsController> construct = () =>
            new(mapper, null!);

        // Act / Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public async Task Details_WhenUseCaseResponseIsUnsuccessful_ReturnsNotFound()
    {
        // Arrange
        Mock<IMapper<EstablishmentDetailsModel, EstablishmentDetailsPageViewModel>> mapper = new();

        Mock<IUseCase<GetEstablishmentByUrnRequest, UseCaseResponse<EstablishmentDetailsReadModel>>> useCase = new();

        UseCaseResponse<EstablishmentDetailsReadModel> useCaseResponse =
            UseCaseResponse<EstablishmentDetailsReadModel>.Failure(
                model: EstablishmentDetailsReadModelTestDoubles.WithEstablishment(null),
                error: "error");

        useCase
            .Setup(x => x.HandleRequestAsync(It.IsAny<GetEstablishmentByUrnRequest>()))
            .ReturnsAsync(useCaseResponse);

        EstablishmentsController controller = new(mapper.Object, useCase.Object);

        // Act
        IActionResult result = await controller.Details(urn: "123456");

        // Assert
        Assert.IsType<NotFoundResult>(result);

        mapper.Verify(
            x => x.Map(It.IsAny<EstablishmentDetailsModel>()),
            Times.Never);
    }

    [Fact]
    public async Task Details_WhenEstablishmentIsNull_ReturnsNotFound()
    {
        // Arrange
        Mock<IMapper<EstablishmentDetailsModel, EstablishmentDetailsPageViewModel>> mapper = new();

        Mock<IUseCase<GetEstablishmentByUrnRequest, UseCaseResponse<EstablishmentDetailsReadModel>>> useCase = new();

        UseCaseResponse<EstablishmentDetailsReadModel> useCaseResponse =
            UseCaseResponse<EstablishmentDetailsReadModel>.Success(
                model: EstablishmentDetailsReadModelTestDoubles.WithEstablishment(null));

        useCase
            .Setup(x => x.HandleRequestAsync(It.IsAny<GetEstablishmentByUrnRequest>()))
            .ReturnsAsync(useCaseResponse);

        EstablishmentsController controller = new(mapper.Object, useCase.Object);

        // Act
        IActionResult result = await controller.Details(urn: "123456");

        // Assert
        Assert.IsType<NotFoundResult>(result);

        mapper.Verify(
            x => x.Map(It.IsAny<EstablishmentDetailsModel>()),
            Times.Never);
    }

    [Fact]
    public async Task Details_WhenEstablishmentExists_ReturnsViewResultWithMappedModel()
    {
        // Arrange
        Mock<IMapper<EstablishmentDetailsModel, EstablishmentDetailsPageViewModel>> mapper = new();

        Mock<IUseCase<GetEstablishmentByUrnRequest, UseCaseResponse<EstablishmentDetailsReadModel>>> useCase = new();

        EstablishmentDetailsModel establishment =
            EstablishmentDetailsBuilder.Create().Build();

        EstablishmentDetailsPageViewModel responseStub =
            EstablishmentDetailsPageViewModelTestDoubles.Stub();

        UseCaseResponse<EstablishmentDetailsReadModel> useCaseResponse =
            UseCaseResponse<EstablishmentDetailsReadModel>.Success(
                model: EstablishmentDetailsReadModelTestDoubles.WithEstablishment(establishment));

        useCase
            .Setup(x => x.HandleRequestAsync(It.IsAny<GetEstablishmentByUrnRequest>()))
            .ReturnsAsync(useCaseResponse);

        mapper
            .Setup(x => x.Map(establishment))
            .Returns(responseStub);

        EstablishmentsController controller =
            new(mapper.Object, useCase.Object);

        // Act
        IActionResult result = await controller.Details("123456");

        // Assert
        ViewResult viewResult = Assert.IsType<ViewResult>(result);

        Assert.Same(responseStub, viewResult.Model);

        mapper.Verify(
            x => x.Map(establishment),
            Times.Once);
    }

    [Fact]
    public async Task Details_PassesUrnToUseCase()
    {
        // Arrange
        Mock<IMapper<EstablishmentDetailsModel, EstablishmentDetailsPageViewModel>> mapper = new();

        Mock<IUseCase<GetEstablishmentByUrnRequest, UseCaseResponse<EstablishmentDetailsReadModel>>> useCase =
            new();

        GetEstablishmentByUrnRequest? capturedRequest = null;

        UseCaseResponse<EstablishmentDetailsReadModel> useCaseResponse =
            UseCaseResponse<EstablishmentDetailsReadModel>.Success(
                model: EstablishmentDetailsReadModelTestDoubles.WithEstablishment(
                    response: EstablishmentDetailsBuilder.Create().Build()));

        useCase
            .Setup(x => x.HandleRequestAsync(It.IsAny<GetEstablishmentByUrnRequest>()))
            .Callback<GetEstablishmentByUrnRequest, CancellationToken>((request, ct) => capturedRequest = request)
            .ReturnsAsync(useCaseResponse);

        mapper
            .Setup(x => x.Map(It.IsAny<EstablishmentDetailsModel>()))
            .Returns(new EstablishmentDetailsPageViewModel());

        EstablishmentsController controller =
            new(mapper.Object, useCase.Object);

        // Act
        await controller.Details("123456");

        // Assert
        Assert.NotNull(capturedRequest);
        Assert.Equal("123456", capturedRequest.Urn);
    }
}
