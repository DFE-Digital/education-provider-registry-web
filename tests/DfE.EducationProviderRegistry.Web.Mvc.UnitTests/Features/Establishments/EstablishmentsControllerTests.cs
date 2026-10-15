using System;
using System.Collections.Generic;
using System.Text;
using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.Core.Libraries.Testing.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishmentById;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.UnitTests.TestDoubles;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Establishments;

public sealed class EstablishmentsControllerTests
{
    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenMapperIsNull()
    {
        Func<EstablishmentsController> construct =
            () => new(
                basicMapper: null!,
                getEstablishmentByIdUseCase: IUseCaseTestDoubles
                    .Default<GetEstablishmentByIdRequest,
                        UseCaseResponse<EstablishmentDetailsReadModel>>());

        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenUseCaseIsNull()
    {
        Func<EstablishmentsController> construct =
            () => new(
                basicMapper:
                    IMapperTestDouble.Default<
                        EstablishmentDetailsModel,
                        EstablishmentDetailsPageViewModel>(),
                getEstablishmentByIdUseCase: null!);

        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public async Task Details_ReturnsNotFound_WhenRequestFails()
    {
        EstablishmentDetailsReadModel response =
            EstablishmentDetailsReadModelTestDoubles.Failure();

        EstablishmentsController controller =
            CreateSut(
                useCase:
                    IUseCaseTestDoubles.WithResponse<
                        GetEstablishmentByIdRequest,
                        EstablishmentDetailsReadModel>(
                        UseCaseResponse<EstablishmentDetailsReadModel>
                            .Failure(response, "error")));

        IActionResult result =
            await controller.Details("123456");

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_ReturnsNotFound_WhenEstablishmentMissing()
    {
        EstablishmentsController controller =
            CreateSut(
                useCase:
                    IUseCaseTestDoubles.WithResponse<
                        GetEstablishmentByIdRequest,
                        EstablishmentDetailsReadModel>(
                        UseCaseResponse<EstablishmentDetailsReadModel>
                            .Success(
                                EstablishmentDetailsReadModelTestDoubles.Failure())));

        IActionResult result =
            await controller.Details("123456");

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_ReturnsView_WithMappedModel_WhenSuccessful()
    {
        EstablishmentDetailsPageViewModel viewModel =
            new()
            {
                Heading = "Academy A"
            };

        Mock<IMapper<
            EstablishmentDetailsModel,
            EstablishmentDetailsPageViewModel>> mapper =
                IMapperTestDouble.Map<
                    EstablishmentDetailsModel,
                    EstablishmentDetailsPageViewModel>(
                    viewModel);

        EstablishmentsController controller =
            CreateSut(
                useCase:
                    IUseCaseTestDoubles.WithResponse<
                        GetEstablishmentByIdRequest,
                        EstablishmentDetailsReadModel>(
                        UseCaseResponse<EstablishmentDetailsReadModel>
                            .Success(
                                EstablishmentDetailsReadModelTestDoubles.Stub())),
                mapper: mapper.Object);

        IActionResult result =
            await controller.Details("123456");

        ViewResult view =
            Assert.IsType<ViewResult>(result);

        Assert.Same(
            viewModel,
            view.Model);
    }

    private static EstablishmentsController CreateSut(
        IMapper<EstablishmentDetailsModel,
            EstablishmentDetailsPageViewModel>? mapper = null,
        IUseCase<GetEstablishmentByIdRequest,
            UseCaseResponse<EstablishmentDetailsReadModel>>? useCase = null)
    {
        return new EstablishmentsController(
            mapper ??
            IMapperTestDouble.Default<
                EstablishmentDetailsModel,
                EstablishmentDetailsPageViewModel>(),
            useCase ??
            IUseCaseTestDoubles.Default<
                GetEstablishmentByIdRequest,
                UseCaseResponse<EstablishmentDetailsReadModel>>());
    }
}
