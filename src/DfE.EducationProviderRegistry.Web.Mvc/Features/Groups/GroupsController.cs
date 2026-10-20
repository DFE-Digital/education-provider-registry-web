using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Groups;

[Route("[controller]")]
public class GroupsController : Controller
{
    private readonly ILogger<GroupsController> _logger;
    private readonly IUseCase<GetGroupByGroupUidRequest, UseCaseResponse<GroupReadModelResponse>> _useCase;
    private readonly IMapper<GroupReadModel, GroupDetailsPageViewModel> _groupDetailsPageMapper;

    public GroupsController(
        ILogger<GroupsController> logger,
        IUseCase<GetGroupByGroupUidRequest, UseCaseResponse<GroupReadModelResponse>> useCase,
        IMapper<GroupReadModel, GroupDetailsPageViewModel> mapper)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(useCase);
        ArgumentNullException.ThrowIfNull(mapper);
        _logger = logger;
        _useCase = useCase;
        _groupDetailsPageMapper = mapper;
    }

    [HttpGet("{groupUid}", Name = "GetGroupByGroupUid")]
    public async Task<IActionResult> Details(string groupUid)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state for groupUid {GroupUId}", groupUid);
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Fetching group details for {GroupUId}", groupUid);

        UseCaseResponse<GroupReadModelResponse> response =
            await _useCase.HandleRequestAsync(
                new GetGroupByGroupUidRequest(groupUid));

        if (!response.SuccessfulRequest)
        {
            _logger.LogError(
                "Use case failed for groupUid {GroupId}",
                groupUid);

            return StatusCode(500);
        }

        if (response.Model.Group is null)
        {
            _logger.LogError(
                "Group not found for groupUid {GroupId}",
                groupUid);

            return NotFound();
        }

        GroupDetailsPageViewModel model = _groupDetailsPageMapper.Map(response.Model.Group);

        return View(model);
    }
}
