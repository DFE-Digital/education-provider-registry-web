using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;
using DfE.EducationProviderRegistry.Web.Mvc.ViewModels.Components;
using DfE.EducationProviderRegistry.Web.Mvc.ViewModels.Pages;

namespace DfE.EducationProviderRegistry.Web.Mvc.Mappers
{
    public class GovUkTableFromEstablishmentSearchResultMapper :
        IMapper<EstablishmentSearchResultViewModel, GovUkTable>
    {
        public GovUkTable Map(EstablishmentSearchResultViewModel result)
        {
            GovUkTableBuilder builder = GovUkTableBuilder
                .Create()
                .WithCaption(result.Name, result.EstablishmentUrl);

            builder.AddRow(
                new GovUkTableCell { Text = "URN", IsBold = true },
                new GovUkTableCell { Text = result.Urn }
            );

            builder.AddRow(
                new GovUkTableCell { Text = "Type", IsBold = true },
                new GovUkTableCell { Text = result.Type }
            );

            builder.AddRow(
                new GovUkTableCell { Text = "Address", IsBold = true },
                new GovUkTableCell { Text = result.Address }
            );

            builder.AddRow(
                new GovUkTableCell { Text = "Local authority", IsBold = true },
                new GovUkTableCell { Text = result.LocalAuthorityName, LinkUrl = result.LocalAuthorityUrl }
            );

            if (result.HasPartOf)
            {
                builder.AddRow(
                    new GovUkTableCell { Text = "Part of", IsBold = true },
                    new GovUkTableCell { Text = result.PartOfName, LinkUrl = result.PartOfUrl }
                );
            }

            return builder.Build();
        }
    }




    public class GovUkTableFromEstablishmentBasicDetailsMapper :
        IMapper<EstablishmentBasicDetailsViewModel, GovUkTable>
    {
        public GovUkTable Map(EstablishmentBasicDetailsViewModel result)
        {
            var builder = GovUkTableBuilder
                .Create();

            builder.AddRow(new GovUkTableCell { Text = "URN", IsBold = true },
                           new GovUkTableCell { Text = result.Urn });

            builder.AddRow(new GovUkTableCell { Text = "UKPRN", IsBold = true },
                           new GovUkTableCell { Text = result.Ukprn });

            builder.AddRow(new GovUkTableCell { Text = "DfE number", IsBold = true },
                           new GovUkTableCell { Text = result.DfeNumber });

            builder.AddRow(new GovUkTableCell { Text = "Status", IsBold = true },
                           new GovUkTableCell { Text = result.Status });

            builder.AddRow(new GovUkTableCell { Text = "Address", IsBold = true },
                           new GovUkTableCell { Text = result.Address });

            builder.AddRow(new GovUkTableCell { Text = "Local authority", IsBold = true },
                           new GovUkTableCell { Text = result.LocalAuthority, LinkUrl = result.LocalAuthorityUrl });

            if (result.HasPartOf)
            {
                builder.AddRow(
                    new GovUkTableCell { Text = "Part of", IsBold = true },
                    new GovUkTableCell { Text = result.PartOfName, LinkUrl = result.PartOfUrl }
                );
            }

            builder.AddRow(new GovUkTableCell { Text = "Type", IsBold = true },
                           new GovUkTableCell { Text = result.Type });

            builder.AddRow(new GovUkTableCell { Text = "Phase of education", IsBold = true },
                           new GovUkTableCell { Text = result.PhaseOfEducation });

            builder.AddRow(new GovUkTableCell { Text = "Age range", IsBold = true },
                           new GovUkTableCell { Text = result.AgeRange });

            builder.AddRow(new GovUkTableCell { Text = "Gender", IsBold = true },
                           new GovUkTableCell { Text = result.Gender });

            builder.AddRow(new GovUkTableCell { Text = "Religious character", IsBold = true },
                           new GovUkTableCell { Text = result.Religiouscharacter });

            builder.AddRow(new GovUkTableCell { Text = "Ofsted rating", IsBold = true },
                           new GovUkTableCell { Text = result.OfstedLastReported, LinkUrl = result.OfstedLastReportedUrl });

            return builder.Build();
        }
    }

    public class GovUkTableFromEstablishmentGovernorMapper :
        IMapper<List<EstablishmentGovernorViewModel>, GovUkTable>
    {
        public GovUkTable Map(List<EstablishmentGovernorViewModel> governors)
        {
            GovUkTableBuilder builder = GovUkTableBuilder
                .Create()
                .WithCaption("Governors")
                .WithHeaders("Name", "Governor ID", "Start date");

            foreach (var g in governors)
            {
                builder.AddRow(
                    new GovUkTableCell { Text = g.Name },
                    new GovUkTableCell { Text = g.GovernorId },
                    new GovUkTableCell { Text = g.StartDate }
                );
            }

            return builder.Build();
        }
    }

    public class GovUkTableFromEstablishmentHistoryMapper :
        IMapper<List<EstablishmentHistoryViewModel>, GovUkTable>
    {
        public GovUkTable Map(List<EstablishmentHistoryViewModel> history)
        {
            GovUkTableBuilder builder = GovUkTableBuilder
                .Create()
                .WithCaption("History");

            foreach (var h in history)
            {
                builder.AddRow(
                    new GovUkTableCell { Text = h.HistoricName, LinkUrl = h.HistoricUrl },
                    new GovUkTableCell { Text = h.HistoricStatus },
                    new GovUkTableCell { Text = $"URN {h.HistoricUrn}" }
                );
            }

            return builder.Build();
        }
    }

}
