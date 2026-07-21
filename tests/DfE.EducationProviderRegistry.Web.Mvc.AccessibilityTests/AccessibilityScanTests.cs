using Deque.AxeCore.Commons;
using Deque.AxeCore.Selenium;
using DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests.Options;
using Docker.DotNet.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Security.AccessControl;
using System.Text;

namespace DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests;

public sealed class AccessibilityScanTests
{
    private readonly CancellationToken _ct;
    private readonly ChromeOptions _chromeOptions;
    private readonly AccessibilityTestOptions _accessibilityTestOptions;
    private readonly ApplicationHostedEnvironment _hostedEnvironment;

    public AccessibilityScanTests(AccessibilityTestOptions options, ApplicationHostedEnvironment hostedEnvironment)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(hostedEnvironment);
        _accessibilityTestOptions = options;
        _hostedEnvironment = hostedEnvironment;
        _ct = TestContext.Current.CancellationToken;

        _chromeOptions = new();

        _chromeOptions.AddArguments(
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
    }

    [Fact]
    public void Axe_Detects_Known_Violation()
    {
        using ChromeDriverService service = ChromeDriverService.CreateDefaultService();
        using IWebDriver driver = new ChromeDriver(service, _chromeOptions);

        driver.Navigate().GoToUrl(
            "data:text/html;charset=utf-8," +
            Uri.EscapeDataString("""
                <!DOCTYPE html>
                <html>
                <body>
                    <img src="logo.png"></img>
                </body>
                </html>
                """));

        AxeResult results = ExecuteScan(driver, _accessibilityTestOptions);

        Assert.Contains(
            results.Violations,
            violation => violation.Id == "image-alt");
    }

    [Theory]
    [MemberData(nameof(AccessibilityScanConfigurationKeys))]
    public async Task Scanned_Page_For_Accessibility_Violations(string configurationKey)
    {
        // TODO capture container logs and output to xunit3, which outputs to console for ci?
        await _hostedEnvironment.InitialiseAsync(_ct);

        if (!_accessibilityTestOptions.Scans.TryGetValue(configurationKey, out AccessibilityTest? test))
        {
            throw new ArgumentException($"Unable to find {configurationKey} in {nameof(AccessibilityTestOptions)}");
        }

        using ChromeDriverService service = ChromeDriverService.CreateDefaultService();
        using IWebDriver? driver = new ChromeDriver(service, _chromeOptions);

        Uri absoluteScanUri = new(
            baseUri: _hostedEnvironment.GetApplicationUrl(),
            relativeUri: test.Route);

        await driver!.Navigate().GoToUrlAsync(absoluteScanUri);

        // TODO Verify request successful and not on error page

        AxeResult results = ExecuteScan(driver, _accessibilityTestOptions);

        string outputDirectory = GetScanOutputDirectory(_accessibilityTestOptions, configurationKey);

        TestContext.Current.AddAttachment(configurationKey, results.ToString());

        byte[] content = Encoding.UTF8.GetBytes(results.ToString());

        await File.WriteAllBytesAsync(
            Path.Combine(outputDirectory, "axe-result.json"),
            content,
            _ct);

        if (results.Violations.Length != 0)
        {
            ((ITakesScreenshot)driver)
                .GetScreenshot()
                .SaveAsFile(Path.Combine(outputDirectory, $"{configurationKey}-screenshot"));
        }

        Assert.True(results.Violations.Length == 0, $"Route {absoluteScanUri} has violations.");
    }

    private static AxeResult ExecuteScan(IWebDriver driver, AccessibilityTestOptions options)
    {
        AxeBuilder axeBuilder = new(driver);

        if (options.WcagTags != null && options.WcagTags.Length > 0)
        {
            axeBuilder.WithTags(options.WcagTags);
        }

        AxeResult accessibilityResults = axeBuilder.Analyze();
        return accessibilityResults;
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