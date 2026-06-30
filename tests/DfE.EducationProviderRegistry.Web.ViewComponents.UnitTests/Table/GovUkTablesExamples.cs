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
            ["January", "£95"],
            ["February", "£55"],
            ["March", "£125"]
        ],
        caption: "Months and rates"
    );

    public static GovUkTable DatesAndAmounts() => new(
        columns: [
            new GovUkTableColumn("Date")
            {
                IsRowHeader = true
            },
            new GovUkTableColumn("Amount")
        ],
        rows: [
            ["First 6 weeks", "£109.80 per week"],
            ["Next 33 weeks", "£109.80 per week"],
            ["Total estimated pay", "£4,282.20"]
        ],
        caption: "Dates and amounts"
    );


    public static GovUkTable RatesComparison() => new(
        columns: [
            new GovUkTableColumn("Month")
            {
                IsRowHeader = true
            },
            new GovUkTableColumn("Rate for bicycles")
            {
                IsNumeric = true
            },
            new GovUkTableColumn("Rate for vehicles")
            {
                IsNumeric = true
            }
        ],
        rows: [
            ["January", "£85", "£95"],
            ["February", "£75", "£55"],
            ["March", "£165", "£125"]
        ],
        caption: "Months and rates"
    );

    public static GovUkTable NoCaption() => new(
        columns: [
            new GovUkTableColumn("City")
            {
                IsRowHeader = true
            },
            new GovUkTableColumn("Population")
            {
                IsNumeric = true
            }
        ],
        rows: [
            ["London", "8.9m"],
            ["Manchester", "553k"]
        ]
    );

    public static GovUkTable LargeCaption() => new(
        columns: [
            new GovUkTableColumn("City")
            {
                IsRowHeader = true
            },
            new GovUkTableColumn("Population")
            {
                IsNumeric = true
            }
        ],
        rows: [
             ["London", "8.9m"],
            ["Manchester", "553k"]
        ],
        caption: "Population",
        captionSize: GovUkCaptionSize.Large
    );

}