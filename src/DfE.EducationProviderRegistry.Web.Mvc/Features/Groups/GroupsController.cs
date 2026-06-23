using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Groups;

[Route("[controller]")]
public class GroupsController : Controller
{
    private readonly ILogger<GroupsController> _logger;
    private readonly IUseCase<GetGroupByGroupIdRequest, UseCaseResponse<GroupReadModel>> _useCase;
    private readonly IMapper<GroupReadModel, GroupDetailsPageViewModel> _groupDetailsPageMapper;

    public GroupsController(
        ILogger<GroupsController> logger,
        IUseCase<GetGroupByGroupIdRequest, UseCaseResponse<GroupReadModel>> useCase,
        IMapper<GroupReadModel, GroupDetailsPageViewModel> mapper)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(useCase);
        ArgumentNullException.ThrowIfNull(mapper);

        _logger = logger;
        _useCase = useCase;
        _groupDetailsPageMapper = mapper;
    }

    [HttpGet("{groupId}", Name = "GetGroupByGroupId")]
    public async Task<IActionResult> Details(string groupId)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state for groupId {GroupId}", groupId);
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Fetching group details for {GroupId}", groupId);

        UseCaseResponse<GroupReadModel> response =
            await _useCase.HandleRequestAsync(
                new GetGroupByGroupIdRequest(groupId));

        if (!response.SuccessfulRequest)
        {
            _logger.LogError(
                "Use case failed for groupId {GroupId}",
                groupId);

            return StatusCode(500);
        }

        if (!response.HasModel())
        {
            _logger.LogError(
                "Group not found for groupId {GroupId}",
                groupId);

            return NotFound();
        }

        GroupDetailsPageViewModel model = _groupDetailsPageMapper.Map(response.Model!);

        return View(model);
    }
}
