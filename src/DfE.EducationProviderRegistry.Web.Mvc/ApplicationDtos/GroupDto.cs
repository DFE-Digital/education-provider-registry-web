namespace DfE.EducationProviderRegistry.Web.Mvc.ApplicationDtos;

public class GroupDto
{
    public GroupBasicDetailsDto BasicDetails { get; set; }
    public List<GroupAcademiesDto> Academies { get; set; }
    public List<GroupTrusteesDto> Trustees { get; set; }
    public List<GroupMembersDto> Members { get; set; }
}

public class GroupBasicDetailsDto
{
    public string Name { get; set; }
    public string Uid { get; set; }
    public string GroupId { get; set; }
    public string Ukprn { get; set; }
    public string CompaniesHouseNumber { get; set; }
    public string Status { get; set; }
    public string Address { get; set; }
    public string Type { get; set; }
}

public class GroupAcademiesDto
{
    public string Name { get; set; }
    public string Urn { get; set; }
}

public class GroupTrusteesDto
{
    public string Name { get; set; }
    public string GovernorId { get; set; }
    public string? Role { get; set; }
    public string StartDate { get; set; }
}

public class GroupMembersDto
{
    public string Name { get; set; }
    public string GovernorId { get; set; }
    public string StartDate { get; set; }
}
