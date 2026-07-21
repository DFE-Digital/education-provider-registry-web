using Deque.AxeCore.Commons;
using Deque.AxeCore.Selenium;
using DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text;

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
    [MemberData(nameof(AccessibilityScanConfigurationKeys))]
    public async Task Scanned_Page_For_Accessibility_Violations(string configurationKey)
    {
        // TODO write a test that should fail with violations

        // TODO capture container logs and output to xunit3, which outputs to console for ci?
        await _hostedEnvironment.InitialiseAsync(_ct);

        if (!_accessibilityTestOptions.Scans.TryGetValue(configurationKey, out AccessibilityTest? test))
        {
            throw new ArgumentException($"Unable to find {configurationKey} in {nameof(AccessibilityTestOptions)}");
        }

        using ChromeDriverService service = ChromeDriverService.CreateDefaultService();
        ChromeOptions options = new();
        options.AddArguments(
            "--incognito",
            // https://github.com/SeleniumHQ/selenium/issues/6049 observed on ubuntu 22.04 runners
            "--disable-dev-shm-usage",
            "--disable-gpu",
            // screen size setting
            "--window-size=1920,1080",
            "--start-maximized",
            "--start-fullscreen",
            // see https://www.selenium.dev/blog/2023/headless-is-going-away/
            "--headless=new",
            // Bypass localhost certificate errors in CI
            "--allow-insecure-localhost");

        using IWebDriver? driver = new ChromeDriver(service, options);

        Uri absoluteScanUri = new(
            baseUri: _hostedEnvironment.GetApplicationUrl(),
            relativeUri: test.Route);

        await driver!.Navigate().GoToUrlAsync(absoluteScanUri);

        // TODO Verify request successful and not on error page

        AxeBuilder axeBuilder = new(driver);

        if (_accessibilityTestOptions.WcagTags != null && _accessibilityTestOptions.WcagTags.Length > 0)
        {
            axeBuilder.WithTags(_accessibilityTestOptions.WcagTags);
        }

        AxeResult accessibilityResults = axeBuilder.Analyze();

        string outputDirectory = GetScanOutputDirectory(_accessibilityTestOptions, configurationKey);

        TestContext.Current.AddAttachment(configurationKey, accessibilityResults.ToString());

        byte[] content = Encoding.UTF8.GetBytes(accessibilityResults.ToString());

        await File.WriteAllBytesAsync(
            Path.Combine(outputDirectory, "axe-result.json"),
            content,
            _ct);

        if (accessibilityResults.Violations.Length != 0)
        {
            ((ITakesScreenshot)driver)
                .GetScreenshot()
                .SaveAsFile(Path.Combine(outputDirectory, $"{configurationKey}-screenshot"));
        }

        Assert.True(accessibilityResults.Violations.Length == 0, $"Route {absoluteScanUri} has violations.");
    }

    private static string GetScanOutputDirectory(AccessibilityTestOptions options, string scanName)
    {
        string path = Path.Combine(
            options.ArtifactOutputDirectory,
            scanName);

        Directory.CreateDirectory(path);

        return path;
    }

    public static TheoryData<string> AccessibilityScanConfigurationKeys = [
        "Home",
        "Group_ById",
        "Establishment_ById"
    ];
}