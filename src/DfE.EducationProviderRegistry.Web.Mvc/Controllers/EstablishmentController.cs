using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishmentById;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Controllers;

public class EstablishmentController : Controller
{
    private readonly IMapper<EstablishmentDetailsModel, EstablishmentDetailsPageViewModel>
        _establishmentDetailsPageMapper;
    private readonly IUseCase<GetEstablishmentByIdRequest, UseCaseResponse<EstablishmentDetailsModel>>
        _getEstablishmentByIdUseCase;

    public EstablishmentController(
        IMapper<EstablishmentDetailsModel, EstablishmentDetailsPageViewModel> basicMapper,
        IUseCase<GetEstablishmentByIdRequest, UseCaseResponse<EstablishmentDetailsModel>> getEstablishmentByIdUseCase)
    {
        ArgumentNullException.ThrowIfNull(basicMapper);
        ArgumentNullException.ThrowIfNull(getEstablishmentByIdUseCase);
        _establishmentDetailsPageMapper = basicMapper;
        _getEstablishmentByIdUseCase = getEstablishmentByIdUseCase;
    }

    [HttpGet("/establishment/{urn}")]
    public async Task<IActionResult> Details(string urn)
    {
        UseCaseResponse<EstablishmentDetailsModel> response = await _getEstablishmentByIdUseCase
            .HandleRequestAsync(new GetEstablishmentByIdRequest(urn));

        // TODO: how do we want to handle unsuccessful responses vs null models?
        if (!response.SuccessfulRequest || response.Model is null)
        {
            return NotFound();
        }

        EstablishmentDetailsPageViewModel model = _establishmentDetailsPageMapper.Map(response.Model);

        return View(model);
    }
}