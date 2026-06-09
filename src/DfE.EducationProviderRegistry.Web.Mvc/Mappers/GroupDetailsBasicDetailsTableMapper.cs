using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Web.Mvc.ApplicationDtos;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;

namespace DfE.EducationProviderRegistry.Web.Mvc.Mappers;

public class GroupDetailsBasicDetailsTableMapper :
    IMapper<GroupBasicDetailsDto, GovUkTable>
{
    public GovUkTable Map(GroupBasicDetailsDto dto)
    {
        GovUkTableBuilder builder = GovUkTableBuilder.Create();

        builder.AddRow(
            new GovUkTableCell { Text = "UID", IsBold = true },
            new GovUkTableCell { Text = dto.Uid }
        );
        builder.AddRow(
            new GovUkTableCell { Text = "Group ID", IsBold = true },
            new GovUkTableCell { Text = dto.GroupId }
        );
        builder.AddRow(
            new GovUkTableCell { Text = "UKPRN", IsBold = true },
            new GovUkTableCell { Text = dto.Ukprn }
        );
        builder.AddRow(
            new GovUkTableCell { Text = "Companies House No.", IsBold = true },
            new GovUkTableCell { Text = dto.CompaniesHouseNumber }
        );
        builder.AddRow(
            new GovUkTableCell { Text = "Status", IsBold = true },
            new GovUkTableCell { Text = dto.Status }
        );
        builder.AddRow(
            new GovUkTableCell { Text = "Address", IsBold = true },
            new GovUkTableCell { Text = dto.Address }
        );
        builder.AddRow(
            new GovUkTableCell { Text = "Type", IsBold = true },
            new GovUkTableCell { Text = dto.Type }
        );

        return builder.Build();
    }
}

public class GroupDetailsAcademiesTableMapper :
    IMapper<List<GroupAcademiesDto>, GovUkTable>
{
    public GovUkTable Map(List<GroupAcademiesDto> dto)
    {
        GovUkTableBuilder builder = GovUkTableBuilder.Create()
            .WithCaption("Academies")
            .WithHeaders("Name", "URN");

        foreach (GroupAcademiesDto academy in dto)
        {
            builder.AddRow(
                new GovUkTableCell { Text = academy.Name, LinkUrl = $"/establishment/{academy.Urn}" },
                new GovUkTableCell { Text = academy.Urn }
            );
        }
        return builder.Build();
    }
}

public class GroupDetailsTrusteesTableMapper :
    IMapper<List<GroupTrusteesDto>, GovUkTable>
{
    public GovUkTable Map(List<GroupTrusteesDto> dto)
    {
        GovUkTableBuilder builder = GovUkTableBuilder.Create()
            .WithCaption("Trustees")
            .WithHeaders("Name", "Governor ID", "Start date");

        foreach (GroupTrusteesDto trustee in dto)
        {
            builder.AddRow(
                new GovUkTableCell { Text = $"{trustee.Name}" },
                new GovUkTableCell { Text = trustee.GovernorId },
                new GovUkTableCell { Text = trustee.StartDate }
            );
        }
        return builder.Build();
    }
}

public class GroupDetailsMembersTableMapper :
    IMapper<List<GroupMembersDto>, GovUkTable>
{
    public GovUkTable Map(List<GroupMembersDto> dto)
    {
        GovUkTableBuilder builder = GovUkTableBuilder.Create()
            .WithCaption("Members")
            .WithHeaders("Name", "Governor ID", "Start date");

        foreach (GroupMembersDto member in dto)
        {
            builder.AddRow(
                new GovUkTableCell { Text = member.Name },
                new GovUkTableCell { Text = member.GovernorId },
                new GovUkTableCell { Text = member.StartDate }
            );
        }
        return builder.Build();
    }
}
