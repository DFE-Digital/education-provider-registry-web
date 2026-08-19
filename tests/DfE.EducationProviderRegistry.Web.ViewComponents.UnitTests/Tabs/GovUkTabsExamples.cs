using DfE.EducationProviderRegistry.Web.ViewComponents.Table;
using DfE.EducationProviderRegistry.Web.ViewComponents.Tabs;

namespace DfE.EducationProviderRegistry.Web.ViewComponents.UnitTests.Tabs;

internal static class GovUkTabsExamples
{
    public static GovUkTabs PastDayWeekMonthYear() =>
        new(
        [
            new(
                "past-day",
                "Past day",
                [
                    PastDayStatistics()
                ]),

            new(
                "past-week",
                "Past week",
                [
                    PastWeekStatistics()
                ]),

            new(
                "past-month",
                "Past month",
                [
                    PastMonthStatistics()
                ]),

            new(
                "past-year",
                "Past year",
                [
                    PastYearStatistics()
                ])
        ]);

    private static TabContent PastDayStatistics() =>
        new(
            "SharedGovUkTable",
            new GovUkTable(
                columns:
                [
                    new TableColumn { Text = "Case manager", IsRowHeader = true },
                    new TableColumn { Text = "Cases opened", IsNumeric = true },
                    new TableColumn { Text = "Cases closed", IsNumeric = true }
                ],
                rows:
                [
                    new TableRow { Cells = { new TableCell { Text = "David Francis" }, new TableCell { Text = "3" }, new TableCell { Text = "0" } } },
                    new TableRow { Cells = { new TableCell { Text = "Paul Farmer" }, new TableCell { Text = "1" }, new TableCell { Text = "0" } } },
                    new TableRow { Cells = { new TableCell { Text = "Rita Patel" }, new TableCell { Text = "2" }, new TableCell { Text = "0" } } }
                ]));

    private static TabContent PastWeekStatistics() =>
        new(
            "SharedGovUkTable",
            new GovUkTable(
                columns:
                [
                    new TableColumn { Text = "Case manager", IsRowHeader = true },
                    new TableColumn { Text = "Cases opened", IsNumeric = true },
                    new TableColumn { Text = "Cases closed", IsNumeric = true }
                ],
                rows:
                [
                    new TableRow { Cells = { new TableCell { Text = "David Francis" }, new TableCell { Text = "24" }, new TableCell { Text = "18" } } },
                    new TableRow { Cells = { new TableCell { Text = "Paul Farmer" }, new TableCell { Text = "16" }, new TableCell { Text = "20" } } },
                    new TableRow { Cells = { new TableCell { Text = "Rita Patel" }, new TableCell { Text = "24" }, new TableCell { Text = "27" } } }
                ]));

    private static TabContent PastMonthStatistics() =>
        new(
            "SharedGovUkTable",
            new GovUkTable(
                columns:
                [
                    new TableColumn { Text = "Case manager", IsRowHeader = true },
                    new TableColumn { Text = "Cases opened", IsNumeric = true },
                    new TableColumn { Text = "Cases closed", IsNumeric = true }
                ],
                rows:
                [
                    new TableRow { Cells = { new TableCell { Text = "David Francis" }, new TableCell { Text = "98" }, new TableCell { Text = "95" } } },
                    new TableRow { Cells = { new TableCell { Text = "Paul Farmer" }, new TableCell { Text = "122" }, new TableCell { Text = "131" } } },
                    new TableRow { Cells = { new TableCell { Text = "Rita Patel" }, new TableCell { Text = "126" }, new TableCell { Text = "142" } } },
                ]));

    private static TabContent PastYearStatistics() =>
        new(
            "SharedGovUkTable",
            new GovUkTable(
                columns:
                [
                    new TableColumn { Text = "Case manager", IsRowHeader = true },
                    new TableColumn { Text = "Cases opened", IsNumeric = true },
                    new TableColumn { Text = "Cases closed", IsNumeric = true }
                ],
                rows:
                [

                    new TableRow { Cells = {new TableCell { Text = "No data available" }} },
                    new TableRow { Cells = {new TableCell { Text = "-" }} },
                    new TableRow { Cells = {new TableCell { Text = "-" }} }

                ]));
}