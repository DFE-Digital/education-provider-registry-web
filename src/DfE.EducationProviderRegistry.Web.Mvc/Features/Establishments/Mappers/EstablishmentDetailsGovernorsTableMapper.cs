using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments.Mappers;

public class EstablishmentDetailsGovernorsTableMapper : IMapper<IEnumerable<GovernorModel>, GovUkTable>
{
    public GovUkTable Map(IEnumerable<GovernorModel> dto)
    {
        GovUkTableBuilder builder = GovUkTableBuilder
            .Create()
            .WithCaption("Board of governors")
            .WithColumns(
            new TableColumn { Text = "Name and role" },
            new TableColumn { Text = "Governor ID" },
            new TableColumn { Text = "Appointed by" },
            new TableColumn { Text = "Term" });

        foreach (GovernorModel g in dto)
        {
            builder.AddRow(
                new TableCell { Text = $"{g.Title} {g.Firstname} {g.Lastname} ({g.Role})" },
                new TableCell { Text = g.Identifier.Value },
                new TableCell { Text = g.AppointedBy },
                new TableCell { Text = $"{g.StartDate} to {g.EndDate}" });
        }

        return builder.Build();
    }
}

public class EstablishmentDetailsHistoricalGovernorsTableMapper : IMapper<IEnumerable<GovernorModel>, GovUkTable>
{
    public GovUkTable Map(IEnumerable<GovernorModel> dto)
    {
        GovUkTableBuilder builder = GovUkTableBuilder
            .Create()
            .WithCaption("Governors who have left within the last 12 months\n")
            .WithColumns(
            new TableColumn { Text = "Name and role" },
            new TableColumn { Text = "Governor ID" },
            new TableColumn { Text = "Appointed by" },
            new TableColumn { Text = "Term" });

        foreach (GovernorModel g in dto)
        {
            builder.AddRow(
                new TableCell { Text = $"{g.Title} {g.Firstname} {g.Lastname} ({g.Role})" },
                new TableCell { Text = g.Identifier.Value },
                new TableCell { Text = g.AppointedBy },
                new TableCell { Text = $"{g.StartDate} to {g.EndDate}" });
        }

        return builder.Build();
    }
}
