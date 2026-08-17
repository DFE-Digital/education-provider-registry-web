using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments;

public class EstablishmentDetailsGovernorsTableMapper :
    IMapper<IEnumerable<GovernorModel>, GovUkTable>
{
    public GovUkTable Map(IEnumerable<GovernorModel> dto)
    {
        GovUkTableBuilder builder = GovUkTableBuilder
            .Create()
            .WithCaption("Governors")
            .WithHeaders("Name", "Governor ID", "Start date");

        foreach (GovernorModel g in dto)
        {
            builder.AddRow(
                new GovUkTableCell { Text = g.Name.Value },
                new GovUkTableCell { Text = g.Identifier.Value ?? "" },
                new GovUkTableCell { Text = string.Empty }
            );
        }

        return builder.Build();
    }
}