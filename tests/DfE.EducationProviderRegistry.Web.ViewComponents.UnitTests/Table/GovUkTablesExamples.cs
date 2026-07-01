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

    public static GovUkTable RatesComparison() => new(
        columns:
        [
            new TableColumn("Month") { IsRowHeader = true },
            new TableColumn("Rate for bicycles") { IsNumeric = true },
            new TableColumn("Rate for vehicles") { IsNumeric = true }
        ],
        rows:
        [
            [
                new TableCell { Text = "January" },
                new TableCell { Text = "£85" },
                new TableCell { Text = "£95" }
            ],
            [
                new TableCell { Text = "February" },
                new TableCell { Text = "£75" },
                new TableCell { Text = "£55" }
            ],
            [
                new TableCell { Text = "March" },
                new TableCell { Text = "£165" },
                new TableCell { Text = "£125" }
            ]
        ],
        caption: "Months and rates"
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