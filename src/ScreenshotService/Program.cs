using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using System.Text.Json;

namespace ScreenshotService;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddSingleton<IScreenshotCaptureService, ScreenshotCaptureService>();
            })
            .Build();

        var screenshotService = host.Services.GetRequiredService<IScreenshotCaptureService>();
        var logger = host.Services.GetRequiredService<ILogger<Program>>();

        try
        {
            logger.LogInformation("Starting eShopLite screenshot capture service");

            // Install Playwright browsers if needed
            await InstallPlaywrightBrowsers();

            // Define screenshot scenarios
            var scenarios = new List<ScreenshotScenario>
            {
                new()
                {
                    Name = "HomePage",
                    Url = "https://localhost:7147/",
                    Description = "Main landing page with chat widget",
                    ViewportWidth = 1920,
                    ViewportHeight = 1080,
                    WaitForSelector = ".hero-title",
                    OutputPath = "../../docs/screenshots/01-home-page.png"
                },
                new()
                {
                    Name = "ChatWidget",
                    Url = "https://localhost:7147/",
                    Description = "Chat widget expanded with conversation",
                    ViewportWidth = 1920,
                    ViewportHeight = 1080,
                    WaitForSelector = ".chat-widget",
                    InteractionScript = "await page.locator('.chat-widget').click(); await page.waitForTimeout(1000);",
                    OutputPath = "../../docs/screenshots/02-chat-widget.png"
                },
                new()
                {
                    Name = "ProductCatalog",
                    Url = "https://localhost:7147/products",
                    Description = "Product catalog page",
                    ViewportWidth = 1920,
                    ViewportHeight = 1080,
                    WaitForSelector = ".product-card",
                    OutputPath = "../../docs/screenshots/03-product-catalog.png"
                },
                new()
                {
                    Name = "SearchResults",
                    Url = "https://localhost:7147/search?q=running+shoes",
                    Description = "Search results page with natural language query",
                    ViewportWidth = 1920,
                    ViewportHeight = 1080,
                    WaitForSelector = ".search-results",
                    OutputPath = "../../docs/screenshots/04-search-results.png"
                },
                new()
                {
                    Name = "ChatConversation",
                    Url = "https://localhost:7147/",
                    Description = "Active chat conversation with AI assistant",
                    ViewportWidth = 1920,
                    ViewportHeight = 1080,
                    WaitForSelector = ".chat-widget",
                    InteractionScript = @"
                        await page.locator('.chat-widget').click();
                        await page.waitForTimeout(2000);
                        await page.fill('input[placeholder*=""Type your message""]', 'What running shoes do you recommend for beginners?');
                        await page.click('.send-btn');
                        await page.waitForTimeout(3000);
                    ",
                    OutputPath = "../../docs/screenshots/05-chat-conversation.png"
                },
                new()
                {
                    Name = "HomePage_Mobile",
                    Url = "https://localhost:7147/",
                    Description = "Mobile view of the homepage",
                    ViewportWidth = 390,
                    ViewportHeight = 844,
                    WaitForSelector = ".hero-title",
                    OutputPath = "../../docs/screenshots/06-home-mobile.png"
                }
            };

            // Capture screenshots
            await screenshotService.CaptureScreenshotsAsync(scenarios);

            logger.LogInformation("Screenshot capture completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during screenshot capture");
            Environment.Exit(1);
        }
    }

    private static async Task InstallPlaywrightBrowsers()
    {
        try
        {
            Microsoft.Playwright.Program.Main(["install", "chromium"]);
            await Task.Delay(1000); // Give installation time to complete
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Note: Playwright browser installation may be needed: {ex.Message}");
        }
    }
}

public interface IScreenshotCaptureService
{
    Task CaptureScreenshotsAsync(IEnumerable<ScreenshotScenario> scenarios);
}

public class ScreenshotCaptureService : IScreenshotCaptureService
{
    private readonly ILogger<ScreenshotCaptureService> _logger;

    public ScreenshotCaptureService(ILogger<ScreenshotCaptureService> logger)
    {
        _logger = logger;
    }

    public async Task CaptureScreenshotsAsync(IEnumerable<ScreenshotScenario> scenarios)
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
            Args = new[]
            {
                "--no-sandbox",
                "--disable-dev-shm-usage",
                "--disable-gpu",
                "--disable-web-security",
                "--allow-running-insecure-content",
                "--ignore-certificate-errors"
            }
        });

        foreach (var scenario in scenarios)
        {
            await CaptureScreenshotAsync(browser, scenario);
        }
    }

    private async Task CaptureScreenshotAsync(IBrowser browser, ScreenshotScenario scenario)
    {
        try
        {
            _logger.LogInformation("Capturing screenshot: {ScenarioName} - {Description}", scenario.Name, scenario.Description);

            var context = await browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize
                {
                    Width = scenario.ViewportWidth,
                    Height = scenario.ViewportHeight
                },
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36"
            });

            var page = await context.NewPageAsync();

            // Navigate to the URL
            await page.GotoAsync(scenario.Url, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            // Wait for specific selector if provided
            if (!string.IsNullOrEmpty(scenario.WaitForSelector))
            {
                try
                {
                    await page.WaitForSelectorAsync(scenario.WaitForSelector, new PageWaitForSelectorOptions
                    {
                        Timeout = 10000
                    });
                }
                catch (TimeoutException)
                {
                    _logger.LogWarning("Timeout waiting for selector {Selector} in scenario {ScenarioName}", scenario.WaitForSelector, scenario.Name);
                }
            }

            // Execute interaction script if provided
            if (!string.IsNullOrEmpty(scenario.InteractionScript))
            {
                try
                {
                    await page.EvaluateAsync($@"
                        (async () => {{
                            const page = {{
                                locator: (selector) => {{
                                    const element = document.querySelector(selector);
                                    return {{
                                        click: () => element?.click(),
                                        fill: (value) => {{ if (element) element.value = value; }}
                                    }};
                                }},
                                fill: (selector, value) => {{
                                    const element = document.querySelector(selector);
                                    if (element) element.value = value;
                                }},
                                click: (selector) => {{
                                    const element = document.querySelector(selector);
                                    element?.click();
                                }},
                                waitForTimeout: (ms) => new Promise(resolve => setTimeout(resolve, ms))
                            }};
                            {scenario.InteractionScript.Replace("await page.", "await page.")}
                        }})();
                    ");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error executing interaction script for scenario {ScenarioName}", scenario.Name);
                }
            }

            // Additional wait for any animations or async operations
            await page.WaitForTimeoutAsync(2000);

            // Ensure output directory exists
            var outputDir = Path.GetDirectoryName(scenario.OutputPath);
            if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            // Take screenshot
            await page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = scenario.OutputPath,
                FullPage = scenario.FullPage,
                Type = ScreenshotType.Png,
                Quality = 90
            });

            _logger.LogInformation("Screenshot saved: {OutputPath}", scenario.OutputPath);

            await context.CloseAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error capturing screenshot for scenario {ScenarioName}", scenario.Name);
        }
    }
}

public class ScreenshotScenario
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ViewportWidth { get; set; } = 1920;
    public int ViewportHeight { get; set; } = 1080;
    public string? WaitForSelector { get; set; }
    public string? InteractionScript { get; set; }
    public string OutputPath { get; set; } = string.Empty;
    public bool FullPage { get; set; } = false;
}
