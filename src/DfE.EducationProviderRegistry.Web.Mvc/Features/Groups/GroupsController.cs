using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Breadcrumbs.Services;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Groups;

[Route("[controller]")]
public class GroupsController : Controller
{
    private readonly ILogger<GroupsController> _logger;
    private readonly IUseCase<GetGroupByGroupUniqueIdentifierRequest, UseCaseResponse<GroupReadModelResponse>> _useCase;
    private readonly IMapper<GroupReadModel, GroupDetailsPageViewModel> _groupDetailsPageMapper;
    private readonly IBreadcrumbJourneyService _navigation;
    public GroupsController(
        ILogger<GroupsController> logger,
        IUseCase<GetGroupByGroupUniqueIdentifierRequest, UseCaseResponse<GroupReadModelResponse>> useCase,
        IMapper<GroupReadModel, GroupDetailsPageViewModel> mapper,
        IBreadcrumbJourneyService navigation)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(useCase);
        ArgumentNullException.ThrowIfNull(mapper);
        ArgumentNullException.ThrowIfNull(navigation);
        _logger = logger;
        _useCase = useCase;
        _groupDetailsPageMapper = mapper;
        _navigation = navigation;
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

        UseCaseResponse<GroupReadModelResponse> response =
            await _useCase.HandleRequestAsync(
                new GetGroupByGroupUniqueIdentifierRequest(groupId));

        if (!response.SuccessfulRequest)
        {
            _logger.LogError(
                "Use case failed for groupId {GroupId}",
                groupId);

            return StatusCode(500);
        }

        if (response.Model.Group is null)
        {
            _logger.LogError(
                "Group not found for groupId {GroupId}",
                groupId);

            return NotFound();
        }

        GroupDetailsPageViewModel model = _groupDetailsPageMapper.Map(response.Model.Group);

        _navigation.VisitGroup(
            model.Heading,
            Url.Action(
                nameof(Details),
                "Groups",
                new { groupId })!);

        return View(model);
    }
}
