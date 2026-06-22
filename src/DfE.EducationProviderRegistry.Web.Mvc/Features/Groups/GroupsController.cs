using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases;
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
        IMapper<GroupReadModel, GroupDetailsPageViewModel> groupDetailsMaper)
    {
        ArgumentNullException.ThrowIfNull(groupDetailsMaper);
        ArgumentNullException.ThrowIfNull(useCase);
        _logger = logger;
        _useCase = useCase;
        _groupDetailsPageMapper = groupDetailsMaper;
    }

    [HttpGet("{groupId}", Name = "GetGroupByGroupId")]
    public async Task<IActionResult> Details(string groupId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        GetGroupByGroupIdRequest request = new(groupId);

        UseCaseResponse<GroupReadModel> response = await _useCase.HandleRequestAsync(request);

        if (!response.SuccessfulRequest || !response.HasModel())
        {
            return NotFound();
        }

        GroupDetailsPageViewModel model = _groupDetailsPageMapper.Map(response.Model!);

        return View(model);
    }
}
