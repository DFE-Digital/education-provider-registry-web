using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Web.Mvc.ApplicationDtos;
using DfE.EducationProviderRegistry.Web.Mvc.ViewModels.Pages;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Controllers;

public class EstablishmentController : Controller
{
    private readonly IMapper<EstablishmentDto, EstablishmentDetailsPageViewModel> _establishmentDetailsPageMapper;

    public EstablishmentController(
        IMapper<EstablishmentDto, EstablishmentDetailsPageViewModel> basicMapper)
    {
        _establishmentDetailsPageMapper = basicMapper;
    }

    [HttpGet("/establishment/{urn}")]
    public IActionResult Details(string urn)
    {
        // Call to use case -> returns application result
        // Map application result -> View model
        EstablishmentDto applicationDtoDummy = new()
        {
            BasicDetails = new EstablishmentBasicDetailsDto()
            {
                Name = "St Mary Primary",
                Urn = urn,
                Ukprn = "123456",
                DfeNumber = "001/001",
                Status = "Opened on 1 October 2018",
                Address = "Some address here",
                LocalAuthority = "Birmingham",
                LocalAuthorityCode = "001",
                PartOfName = "The park academies trust",
                PartOfCode = "001",
                Type = " Academy",
                PhaseOfEducation = "Primary",
                AgeRange = "4 to 11",
                Gender = "Mixed",
                ReligiousCharacter = "Roman Catholic",
                OfstedLastReported = "reported on x date",
                OfstedLastReportedUrl = "/"
            },
            Governors = new()
            {
                new EstablishmentGovernorDto()
                {
                    Name = "Elena Vance (chair)",
                    GovernorId = "000001",
                    StartDate = "27 august 2025"
                },
                new EstablishmentGovernorDto()
                {
                    Name = "Julian Cross",
                    GovernorId = "000002",
                    StartDate = "27 september 2025"
                },
                new EstablishmentGovernorDto()
                {
                    Name = "Maya Sterling",
                    GovernorId = "000003",
                    StartDate = "27 october 2025"
                },
                new EstablishmentGovernorDto()
                {
                    Name = "Clara Whitlock",
                    GovernorId = "000001",
                    StartDate = "27 novemeber 2025"
                }
            },
            History = new()
            {
                new EstablishmentHistoryDto()
                {
                    Name = "St marys",
                    Urn = "001",
                    Status = "Maintained"
                }
            }
        };

        EstablishmentDetailsPageViewModel model = _establishmentDetailsPageMapper.Map(applicationDtoDummy);

        return View(model);
    }
}