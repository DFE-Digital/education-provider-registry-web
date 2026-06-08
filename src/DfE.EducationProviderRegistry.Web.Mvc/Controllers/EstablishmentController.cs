using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;
using DfE.EducationProviderRegistry.Web.Mvc.ViewModels.Pages;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Controllers;

public class EstablishmentController : Controller
{
    private readonly IMapper<EstablishmentBasicDetailsViewModel, GovUkTable> _basicMapper;
    private readonly IMapper<List<EstablishmentGovernorViewModel>, GovUkTable> _governorMapper;
    private readonly IMapper<List<EstablishmentHistoryViewModel>, GovUkTable> _historyMapper;

    public EstablishmentController(
        IMapper<EstablishmentBasicDetailsViewModel, GovUkTable> basicMapper,
        IMapper<List<EstablishmentGovernorViewModel>, GovUkTable> governorMapper,
        IMapper<List<EstablishmentHistoryViewModel>, GovUkTable> historyMapper)
    {
        _basicMapper = basicMapper;
        _governorMapper = governorMapper;
        _historyMapper = historyMapper;
    }

    [HttpGet("/establishment/{urn}")]
    public IActionResult Details(string urn)
    {
        // Call to use case -> returns application result
        // Map application result -> View model
        EstablishmentDetailsPageViewModel model = new()
        {
            BasicDetails = new EstablishmentBasicDetailsViewModel()
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
                Religiouscharacter = "Roman Catholic",
                OfstedLastReported = "reported on x date",
                OfstedLastReportedUrl = "/"
            },
            Governors = new()
            {
                new EstablishmentGovernorViewModel()
                {
                    Name = "Elena Vance (chair)",
                    GovernorId = "000001",
                    StartDate = "27 august 2025"
                },
                new EstablishmentGovernorViewModel()
                {
                    Name = "Julian Cross",
                    GovernorId = "000002",
                    StartDate = "27 september 2025"
                },
                new EstablishmentGovernorViewModel()
                {
                    Name = "Maya Sterling",
                    GovernorId = "000003",
                    StartDate = "27 october 2025"
                },
                new EstablishmentGovernorViewModel()
                {
                    Name = "Clara Whitlock",
                    GovernorId = "000001",
                    StartDate = "27 novemeber 2025"
                }
            },
            History = new()
            {
                new EstablishmentHistoryViewModel()
                {
                    HistoricName = "St marys",
                    HistoricUrn = "001",
                    HistoricStatus = "Maintained"
                }
            }
        };

        // Build component models
        model.BasicDetailsTable = _basicMapper.Map(model.BasicDetails);
        model.GovernorsTable = _governorMapper.Map(model.Governors);
        model.HistoryTable = _historyMapper.Map(model.History);

        return View(model);
    }
}