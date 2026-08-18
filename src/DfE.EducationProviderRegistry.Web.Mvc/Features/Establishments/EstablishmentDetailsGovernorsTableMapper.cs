using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments;

public class EstablishmentDetailsGovernorsTableMapper :
    IMapper<IEnumerable<GovernorModel>, GovUkTable>
{
    public GovUkTable Map(IEnumerable<GovernorModel> dto)
    {
        GovUkTableBuilder builder = GovUkTableBuilder
            .Create()
            .WithCaption("Governors")
            .WithColumns(
            new TableColumn ("Name"),
            new TableColumn ("Governor ID"),
            new TableColumn ("Start date")
        );

        foreach (GovernorModel g in dto)
        {
            builder.AddRow(
                new TableCell { Text = g.Name.Value },
                new TableCell { Text = g.Identifier.Value ?? "" },
                new TableCell { Text = string.Empty }
            );
        }

        return builder.Build();
    }
}