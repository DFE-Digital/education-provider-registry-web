using DfE.EducationProviderRegistry.Web.ViewComponents.SummaryList;

namespace DfE.EducationProviderRegistry.Web.ViewComponents.UnitTests.SummaryList;

internal static class GovUkSummaryListExamples
{
    public static GovUkSummaryList Basic() =>
        new(
        [
            new(
                "Name",
                new SummaryListValue("Sarah Philips")),

            new(
                "Date of birth",
                new SummaryListValue("5 January 1978")),

            new(
                "Address",
                new SummaryListValue("72 Guild Street, London, SE23 6FH")),

            new(
                "Contact details",
                new SummaryListValue("07700 900457, sarah.phillips@example.com"))
        ]);

    public static GovUkSummaryList WithActions() =>
        new(
        [
            new(
                "Name",
                new SummaryListValue("Sarah Philips"),
                [
                    new SummaryListAction(
                        "Change",
                        "/change-name")
                ]),

            new(
                "Date of birth",
                new SummaryListValue("5 January 1978"),
                [
                    new SummaryListAction(
                        "Change",
                        "/change-date-of-birth")
                ]),

            new(
                "Address",
                new SummaryListValue("72 Guild Street, London, SE23 6FH"),
                [
                    new SummaryListAction(
                        "Change",
                        "/change-address")
                ]),

            new(
                "Contact details",
                new SummaryListValue("07700 900457, sarah.phillips@example.com"),
                [
                    new SummaryListAction(
                        "Change",
                        "/change-contact-details")
                ])
        ]);

    public static GovUkSummaryList MixedActions() =>
        new(
        [
            new(
                "Name",
                new SummaryListValue("Sarah Philips"),
                [
                    new SummaryListAction(
                        "Change",
                        "/change-name")
                ]),

            new(
                "Date of birth",
                new SummaryListValue("5 January 1978")),

            new(
                "Address",
                new SummaryListValue("72 Guild Street, London, SE23 6FH"),
                [
                    new SummaryListAction(
                        "Change",
                        "/change-address")
                ]),

            new(
                "Contact details",
                new SummaryListValue("07700 900457, sarah.phillips@example.com"))
        ]);

    public static GovUkSummaryList ContactDetails() =>
        new(
        [
            new(
                "Telephone",
                new SummaryListValue("07700 900457")),

            new(
                "Email",
                new SummaryListValue("sarah.phillips@example.com"))
        ]);
}