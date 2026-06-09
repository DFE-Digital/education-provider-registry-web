using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Web.Mvc.ApplicationDtos;
using DfE.EducationProviderRegistry.Web.Mvc.ViewModels.Pages;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.Controllers
{
    public class GroupController : Controller
    {
        private readonly IMapper<GroupDto, GroupDetailsPageViewModel> _groupDetailsPageMapper;

        public GroupController(
            IMapper<GroupDto, GroupDetailsPageViewModel> groupDetailsMaper)
        {
            _groupDetailsPageMapper = groupDetailsMaper;
        }

        [HttpGet("/group/{code}")]
        public IActionResult Details(string code)
        {
            GroupDto applicationDtoDummy = new()
            {
                BasicDetails = new GroupBasicDetailsDto
                {
                    Name = "Erdington Trust",
                    Uid = "T001",
                    GroupId = code,
                    Ukprn = "12345678",
                    CompaniesHouseNumber = "09876543",
                    Status = "Open",
                    Address = "123 Trust Road, Birmingham",
                    Type = "Multi-academy trust"
                },
                Academies = new List<GroupAcademiesDto>
                {
                    new() { Name = "St Mary's Primary", Urn = "100001" },
                    new() { Name = "St John's Academy", Urn = "100002" }
                },
                Trustees = new List<GroupTrusteesDto>
                {
                    new() { Name = "Alice Brown", GovernorId = "G001", Role = "Chair", StartDate = "01 Jan 2020" },
                    new() { Name = "David Smith", GovernorId = "G002", Role = "Trustee", StartDate = "15 Mar 2021" }
                },
                Members = new List<GroupMembersDto>
                {
                    new() { Name = "Sarah Green", GovernorId = "G001", StartDate = "10 Feb 2019" },
                    new() { Name = "John White", GovernorId = "G002", StartDate = "22 Jun 2020" }
                }
            };

            GroupDetailsPageViewModel model = _groupDetailsPageMapper.Map(applicationDtoDummy);

            return View(model);
        }
    }
}
