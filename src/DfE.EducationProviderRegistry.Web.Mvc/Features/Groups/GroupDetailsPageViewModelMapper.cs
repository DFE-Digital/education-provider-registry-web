using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Groups;

internal sealed class GroupDetailsPageViewModelMapper :
    IMapper<GroupReadModel, GroupDetailsPageViewModel>
{
    private readonly IMapper<IEnumerable<Academy>, GovUkTable> _academiesMapper;
    private readonly IMapper<IEnumerable<TrusteeReadModel>, GovUkTable> _trusteesMapper;
    private readonly IMapper<IEnumerable<MemberReadModel>, GovUkTable> _membersMapper;

    public GroupDetailsPageViewModelMapper(
        IMapper<IEnumerable<Academy>, GovUkTable> academiesMapper,
        IMapper<IEnumerable<TrusteeReadModel>, GovUkTable> trusteesMapper,
        IMapper<IEnumerable<MemberReadModel>, GovUkTable> membersMapper)
    {
        _academiesMapper = academiesMapper;
        _trusteesMapper = trusteesMapper;
        _membersMapper = membersMapper;
    }
    public GroupDetailsPageViewModel Map(GroupReadModel readModel)
    {
        return new GroupDetailsPageViewModel
        {
            Heading = readModel.Name,
            BasicDetailsTable = CreateBasicDetails(readModel),
            AcademiesTable = _academiesMapper.Map(readModel.Academies),
            TrusteesTable = _trusteesMapper.Map(readModel.Trustees),
            MembersTable = _membersMapper.Map(readModel.Members)
        };
    }

    private static GovUkTable CreateBasicDetails(GroupReadModel model)
    {
        GovUkTableBuilder builder = GovUkTableBuilder.Create();

        builder.AddRow(
            new GovUkTableCell { Text = "UID", IsBold = true },
            new GovUkTableCell { Text = model.GroupUID.ToString() });

        builder.AddRow(
            new GovUkTableCell { Text = "Group ID", IsBold = true },
            new GovUkTableCell { Text = model.GroupId });

        builder.AddRow(
            new GovUkTableCell { Text = "UKPRN", IsBold = true },
            new GovUkTableCell { Text = model.UKPRN });

        builder.AddRow(
            new GovUkTableCell
            {
                Text = "Companies House No.",
                IsBold = true
            }, 
            new GovUkTableCell
            {
                Text = model.CompaniesHouseId != null ? $"{model.CompaniesHouseId} (opens in new tab)" : string.Empty, // TODO hardcoded new tab text semantics
                LinkUrl = $"https://find-and-update.company-information.service.gov.uk/company/{model.CompaniesHouseId}"
            }
        );

        builder.AddRow(
            new GovUkTableCell { Text = "Status", IsBold = true },
            new GovUkTableCell { Text = model.Status }
        );

        builder.AddRow(
            new GovUkTableCell { Text = "Address", IsBold = true },
            new GovUkTableCell { Text = model.Address }
        );

        builder.AddRow(
            new GovUkTableCell { Text = "Type", IsBold = true },
            new GovUkTableCell { Text = model.Type }
        );
        return builder.Build();
    }
}

internal sealed class GroupDetailsAcademiesTableMapper :
    IMapper<IEnumerable<Academy>, GovUkTable>
{
    public GovUkTable Map(IEnumerable<Academy> input)
    {
        GovUkTableBuilder builder = GovUkTableBuilder.Create()
            .WithCaption("Academies")
            .WithHeaders("Name", "URN");

        foreach (Academy academy in input)
        {
            builder.AddRow(
                new GovUkTableCell
                {
                    Text = academy.Name.ToString(),
                    LinkUrl = $"/establishment/{academy.Id.ToString()}"
                },
                new GovUkTableCell
                {
                    Text = academy.Id.Value.ToString()
                }
            );
        }
        return builder.Build();
    }
}

public class GroupDetailsTrusteesTableMapper :
    IMapper<IEnumerable<TrusteeReadModel>, GovUkTable>
{
    public GovUkTable Map(IEnumerable<TrusteeReadModel> input)
    {
        GovUkTableBuilder builder = GovUkTableBuilder.Create()
            .WithCaption("Trustees")
            .WithHeaders("Name", "Governor ID", "Start date");

        foreach (TrusteeReadModel trustee in input)
        {
            builder.AddRow(
                new GovUkTableCell { Text = $"{trustee.FullName}" },
                new GovUkTableCell { Text = trustee.Id },
                new GovUkTableCell { Text = trustee.StartDate.ToString("dd MMMM yyyy") } // TODO check format
            );
        }
        return builder.Build();
    }
}

public class GroupDetailsMembersTableMapper :
    IMapper<IEnumerable<MemberReadModel>, GovUkTable>
{
    public GovUkTable Map(IEnumerable<MemberReadModel> dto)
    {
        GovUkTableBuilder builder = GovUkTableBuilder.Create()
            .WithCaption("Members")
            .WithHeaders("Name", "Governor ID", "Start date");

        foreach (MemberReadModel member in dto)
        {
            builder.AddRow(
                new GovUkTableCell { Text = member.FullName },
                new GovUkTableCell { Text = member.Identifier },
                new GovUkTableCell { Text = member.StartDate.ToString("dd MMMM yyyy") } // TODO check format and align property values
            );
        }
        return builder.Build();
    }
}
