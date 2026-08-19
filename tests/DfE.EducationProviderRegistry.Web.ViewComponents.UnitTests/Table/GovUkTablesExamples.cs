using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

namespace DfE.EducationProviderRegistry.Web.ViewComponents.UnitTests.Table;

internal static class GovUkTablesExamples
{
    public static GovUkTable MonthsAndRates() => new(
        columns:
        [
            new TableColumn { Text = "Month", IsRowHeader = true },
            new TableColumn { Text = "Rate for vehicles", IsNumeric = true }
        ],
        rows:
        [
            new TableRow{
            Cells =
                [
                    new TableCell { Text = "January" },
                    new TableCell { Text = "£95" }
                ]
            },
            new TableRow{
            Cells =
                [
                    new TableCell { Text = "February" },
                    new TableCell { Text = "£55" }
                ]
            },
            new TableRow{
            Cells =
                [
                    new TableCell { Text = "March" },
                    new TableCell { Text = "£125" }
                ]
            }
        ],
        caption: "Months and rates"
    );

    public static GovUkTable DatesAndAmounts() => new(
        columns:
        [
            new TableColumn { Text = "Date", IsRowHeader = true },
            new TableColumn { Text = "Amount" }
        ],
        rows:
        [
            new TableRow { Cells = { new TableCell { Text = "First 6 weeks" }, new TableCell { Text = "£109.80 per week" } } },
            new TableRow { Cells = { new TableCell { Text = "Next 33 weeks" }, new TableCell { Text = "£109.80 per week" } } },
            new TableRow { Cells = { new TableCell { Text = "Total estimated pay" }, new TableCell { Text = "£4,282.20" } } }
        ],
        caption: "Dates and amounts"
    );

    public static GovUkTable CaseStatistics() => new(
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
        ]
    );

    public static GovUkTable NoRowHeaders() => new(
        columns:
        [
            new TableColumn { Text = "City" },
            new TableColumn { Text = "Population", IsNumeric = true }
        ],
        rows:
        [
            new TableRow { Cells = { new TableCell { Text = "London" }, new TableCell { Text = "8.9" } } },
            new TableRow { Cells = { new TableCell { Text = "Manchester" }, new TableCell { Text = "553" } } }
        ],
        caption: "Cities and population"
    );

    public static GovUkTable NoCaption() => new(
        columns:
        [
            new TableColumn { Text = "City", IsRowHeader = true },
            new TableColumn { Text = "Population", IsNumeric = true }
        ],
        rows:
        [
            new TableRow { Cells = { new TableCell { Text = "London" }, new TableCell { Text = "8.9m" } } },
            new TableRow { Cells = { new TableCell { Text = "Manchester" }, new TableCell { Text = "553k" } } }
        ]
    );

    public static GovUkTable LargeCaption() => new(
        columns:
        [
            new TableColumn { Text = "City", IsRowHeader = true },
            new TableColumn { Text = "Population", IsNumeric = true }
        ],
        rows:
        [
            new TableRow { Cells = { new TableCell { Text = "London" }, new TableCell { Text = "8.9m" } } },
            new TableRow { Cells = { new TableCell { Text = "Manchester" }, new TableCell { Text = "553k" } } }
        ],
        caption: "Population",
        captionSize: TableCaptionSize.Large
    );
}