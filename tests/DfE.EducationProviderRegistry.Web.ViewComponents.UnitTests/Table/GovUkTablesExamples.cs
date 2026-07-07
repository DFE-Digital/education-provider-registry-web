using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

namespace DfE.EducationProviderRegistry.Web.ViewComponents.UnitTests.Table;

internal static class GovUkTablesExamples
{
    public static GovUkTable MonthsAndRates() => new(
        columns:
        [
            new TableColumn("Month") { IsRowHeader = true },
            new TableColumn("Rate for vehicles") { IsNumeric = true }
        ],
        rows:
        [
            [
                new TableCell { Text = "January" },
                new TableCell { Text = "£95" }
            ],
            [
                new TableCell { Text = "February" },
                new TableCell { Text = "£55" }
            ],
            [
                new TableCell { Text = "March" },
                new TableCell { Text = "£125" }
            ]
        ],
        caption: "Months and rates"
    );

    public static GovUkTable DatesAndAmounts() => new(
        columns:
        [
            new TableColumn("Date") { IsRowHeader = true },
            new TableColumn("Amount")
        ],
        rows:
        [
            [
                new TableCell { Text = "First 6 weeks" },
                new TableCell { Text = "£109.80 per week" }
            ],
            [
                new TableCell { Text = "Next 33 weeks" },
                new TableCell { Text = "£109.80 per week" }
            ],
            [
                new TableCell { Text = "Total estimated pay" },
                new TableCell { Text = "£4,282.20" }
            ]
        ],
        caption: "Dates and amounts"
    );

    public static GovUkTable CaseStatistics() => new(
        columns:
        [
            new TableColumn("Case manager") { IsRowHeader = true },
            new TableColumn("Cases opened") { IsNumeric = true },
            new TableColumn("Cases closed") { IsNumeric = true }
        ],
        rows:
        [
            [
                new TableCell { Text = "David Francis" },
                new TableCell { Text = "3" },
                new TableCell { Text = "0" }
            ],
            [
                new TableCell { Text = "Paul Farmer" },
                new TableCell { Text = "1" },
                new TableCell { Text = "0" }
            ],
            [
                new TableCell { Text = "Rita Patel" },
                new TableCell { Text = "2" },
                new TableCell { Text = "0" }
            ]
        ]
    );

    public static GovUkTable NoRowHeaders() => new(
        columns:
        [
            new TableColumn("City"),
            new TableColumn("Population") { IsNumeric = true }
        ],
        rows:
        [
            [
                new TableCell { Text = "London" },
                new TableCell { Text = "8.9" }
            ],
            [
                new TableCell { Text = "Manchester" },
                new TableCell { Text = "553" }
            ]
        ],
        caption: "Cities and population"
    );

    public static GovUkTable NoCaption() => new(
        columns:
        [
            new TableColumn("City") { IsRowHeader = true },
            new TableColumn("Population") { IsNumeric = true }
        ],
        rows:
        [
            [
                new TableCell { Text = "London" },
                new TableCell { Text = "8.9m" }
            ],
            [
                new TableCell { Text = "Manchester" },
                new TableCell { Text = "553k" }
            ]
        ]
    );

    public static GovUkTable LargeCaption() => new(
        columns:
        [
            new TableColumn("City") { IsRowHeader = true },
            new TableColumn("Population") { IsNumeric = true }
        ],
        rows:
        [
            [
                new TableCell { Text = "London" },
                new TableCell { Text = "8.9m" }
            ],
            [
                new TableCell { Text = "Manchester" },
                new TableCell { Text = "553k" }
            ]
        ],
        caption: "Population",
        captionSize: TableCaptionSize.Large
    );
}