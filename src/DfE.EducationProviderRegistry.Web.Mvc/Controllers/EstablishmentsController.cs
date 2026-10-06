using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishmentById;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.Features.NavigationJourney;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Controllers;

[Route("establishments")]
public class EstablishmentsController : Controller
{
    private readonly IMapper<EstablishmentDetailsModel, EstablishmentDetailsPageViewModel> _establishmentDetailsPageMapper;
    private readonly IUseCase<GetEstablishmentByIdRequest, UseCaseResponse<EstablishmentDetailsReadModel>> _getEstablishmentByIdUseCase;
    private readonly INavigationJourneyService _navigation;

    public EstablishmentsController(
        IMapper<EstablishmentDetailsModel, EstablishmentDetailsPageViewModel> basicMapper,
        IUseCase<GetEstablishmentByIdRequest, UseCaseResponse<EstablishmentDetailsReadModel>> getEstablishmentByIdUseCase,
        INavigationJourneyService navigation)
    {
        ArgumentNullException.ThrowIfNull(basicMapper);
        ArgumentNullException.ThrowIfNull(getEstablishmentByIdUseCase);
        ArgumentNullException.ThrowIfNull(navigation);
        _establishmentDetailsPageMapper = basicMapper;
        _getEstablishmentByIdUseCase = getEstablishmentByIdUseCase;
        _navigation = navigation;
    }

    [HttpGet("{urn}")]
    public async Task<IActionResult> Details(string urn)
    {
        UseCaseResponse<EstablishmentDetailsReadModel> response =
            await _getEstablishmentByIdUseCase.HandleRequestAsync(
                new GetEstablishmentByIdRequest(urn));

        // TODO: how do we want to handle unsuccessful responses vs null models?
        if (!response.SuccessfulRequest || response.Model.Establishment is null)
        {
            return NotFound();
        }

        EstablishmentDetailsPageViewModel model = _establishmentDetailsPageMapper.Map(response.Model.Establishment);

        _navigation.EnterEstablishment(
            model.Heading,
            Url.Action(
                nameof(Details),
                "Establishments",
                new { urn })!);

        return View(model);
    }
}
