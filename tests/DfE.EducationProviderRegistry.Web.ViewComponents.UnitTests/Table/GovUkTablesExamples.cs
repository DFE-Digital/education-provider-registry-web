using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

namespace DfE.EducationProviderRegistry.Web.ViewComponents.UnitTests.Table;

internal static class GovUkTablesExamples
{
    public static GovUkTable MonthsAndRates() => new(
        columns:
        [
            new GovUkTableColumn("Month") { IsRowHeader = true },
            new GovUkTableColumn("Rate for vehicles") { IsNumeric = true }
        ],
        rows:
        [
            [
                new GovUkTableCell { Text = "January" },
                new GovUkTableCell { Text = "£95" }
            ],
            [
                new GovUkTableCell { Text = "February" },
                new GovUkTableCell { Text = "£55" }
            ],
            [
                new GovUkTableCell { Text = "March" },
                new GovUkTableCell { Text = "£125" }
            ]
        ],
        caption: "Months and rates"
    );

    public static GovUkTable DatesAndAmounts() => new(
        columns:
        [
            new GovUkTableColumn("Date") { IsRowHeader = true },
            new GovUkTableColumn("Amount")
        ],
        rows:
        [
            [
                new GovUkTableCell { Text = "First 6 weeks" },
                new GovUkTableCell { Text = "£109.80 per week" }
            ],
            [
                new GovUkTableCell { Text = "Next 33 weeks" },
                new GovUkTableCell { Text = "£109.80 per week" }
            ],
            [
                new GovUkTableCell { Text = "Total estimated pay" },
                new GovUkTableCell { Text = "£4,282.20" }
            ]
        ],
        caption: "Dates and amounts"
    );

    public static GovUkTable RatesComparison() => new(
        columns:
        [
            new GovUkTableColumn("Month") { IsRowHeader = true },
            new GovUkTableColumn("Rate for bicycles") { IsNumeric = true },
            new GovUkTableColumn("Rate for vehicles") { IsNumeric = true }
        ],
        rows:
        [
            [
                new GovUkTableCell { Text = "January" },
                new GovUkTableCell { Text = "£85" },
                new GovUkTableCell { Text = "£95" }
            ],
            [
                new GovUkTableCell { Text = "February" },
                new GovUkTableCell { Text = "£75" },
                new GovUkTableCell { Text = "£55" }
            ],
            [
                new GovUkTableCell { Text = "March" },
                new GovUkTableCell { Text = "£165" },
                new GovUkTableCell { Text = "£125" }
            ]
        ],
        caption: "Months and rates"
    );

    public static GovUkTable NoCaption() => new(
        columns:
        [
            new GovUkTableColumn("City") { IsRowHeader = true },
            new GovUkTableColumn("Population") { IsNumeric = true }
        ],
        rows:
        [
            [
                new GovUkTableCell { Text = "London" },
                new GovUkTableCell { Text = "8.9m" }
            ],
            [
                new GovUkTableCell { Text = "Manchester" },
                new GovUkTableCell { Text = "553k" }
            ]
        ]
    );

    public static GovUkTable LargeCaption() => new(
        columns:
        [
            new GovUkTableColumn("City") { IsRowHeader = true },
            new GovUkTableColumn("Population") { IsNumeric = true }
        ],
        rows:
        [
            [
                new GovUkTableCell { Text = "London" },
                new GovUkTableCell { Text = "8.9m" }
            ],
            [
                new GovUkTableCell { Text = "Manchester" },
                new GovUkTableCell { Text = "553k" }
            ]
        ],
        caption: "Population",
        captionSize: GovUkCaptionSize.Large
    );
}