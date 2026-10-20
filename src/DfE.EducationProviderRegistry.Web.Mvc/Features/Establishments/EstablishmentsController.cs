using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishmentByUrn;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments;

[Route("establishments")]
public class EstablishmentsController : Controller
{
    private readonly IMapper<EstablishmentDetailsModel, EstablishmentDetailsPageViewModel> _establishmentDetailsPageMapper;
    private readonly IUseCase<GetEstablishmentByUrnRequest, UseCaseResponse<EstablishmentDetailsReadModel>> _getEstablishmentByIdUseCase;

    public EstablishmentsController(
        IMapper<EstablishmentDetailsModel, EstablishmentDetailsPageViewModel> basicMapper,
        IUseCase<GetEstablishmentByUrnRequest, UseCaseResponse<EstablishmentDetailsReadModel>> getEstablishmentByIdUseCase)
    {
        ArgumentNullException.ThrowIfNull(basicMapper);
        ArgumentNullException.ThrowIfNull(getEstablishmentByIdUseCase);
        _establishmentDetailsPageMapper = basicMapper;
        _getEstablishmentByIdUseCase = getEstablishmentByIdUseCase;
    }

    [HttpGet("{urn}")]
    public async Task<IActionResult> Details(string urn)
    {
        UseCaseResponse<EstablishmentDetailsReadModel> response =
            await _getEstablishmentByIdUseCase.HandleRequestAsync(
                new GetEstablishmentByUrnRequest(urn));

        // TODO: how do we want to handle unsuccessful responses vs null models?
        if (!response.SuccessfulRequest || response.Model.Establishment is null)
        {
            return NotFound();
        }

        EstablishmentDetailsPageViewModel model = _establishmentDetailsPageMapper.Map(response.Model.Establishment);

        return View(model);
    }
}
