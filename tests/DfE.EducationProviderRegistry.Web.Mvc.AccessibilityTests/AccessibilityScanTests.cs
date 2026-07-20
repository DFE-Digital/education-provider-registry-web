using Deque.AxeCore.Commons;
using Deque.AxeCore.Selenium;
using DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests;

public sealed class AccessibilityScanTests
{
    private readonly CancellationToken _ct;
    private readonly AccessibilityTestOptions _accessibilityTestOptions;
    private readonly ApplicationHostedEnvironment _hostedEnvironment;

    public AccessibilityScanTests(AccessibilityTestOptions options, ApplicationHostedEnvironment hostedEnvironment)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(hostedEnvironment);
        _accessibilityTestOptions = options;
        _hostedEnvironment = hostedEnvironment;
        _ct = TestContext.Current.CancellationToken;
    }

    [Theory]
    [MemberData(nameof(AccessibilityConfigurationKeys))]
    public async Task Scanned_Page_For_Accessibility_Violations(string configurationKey)
    {
        await _hostedEnvironment.InitialiseAsync(_ct);

        if(!_accessibilityTestOptions.Tests.TryGetValue(configurationKey, out AccessibilityTest? test))
        {
            throw new ArgumentException($"Unable to find {configurationKey} in {nameof(AccessibilityTestOptions)}");
        }
        
        using ChromeDriverService service = ChromeDriverService.CreateDefaultService();
        using IWebDriver? driver = new ChromeDriver(service, new ChromeOptions());

        Uri absoluteScanUri = new(
            baseUri: _hostedEnvironment.GetApplicationUrl(),
            relativeUri: test.Route);

        await driver!.Navigate().GoToUrlAsync(absoluteScanUri);

        AxeResult accessibilityResults = 
            new AxeBuilder(driver)
                .Analyze();

        TestContext.Current.AddAttachment(configurationKey, accessibilityResults.ToString());

        if (accessibilityResults.Violations.Length != 0)
        {
            ((ITakesScreenshot)driver)
                .GetScreenshot()
                .SaveAsFile($"{configurationKey}-failure");
        }

        Assert.True(accessibilityResults.Violations.Length == 0, $"Route {absoluteScanUri} has violations.");
    }

    public static TheoryData<string> AccessibilityConfigurationKeys = [
        "Home",
        "Group_ById",
        "Establishment_ById"
    ];
}