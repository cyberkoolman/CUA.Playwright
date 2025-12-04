using Microsoft.Playwright;

class ProgramSimple
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Computer Use Agent with Playwright ===\n");
        Console.WriteLine("This demo shows Playwright browser automation combined with screenshot capture");
        Console.WriteLine("for AI-powered Computer Use scenarios.\n");

        // Install Playwright browsers if needed
        Console.WriteLine("Ensuring Playwright browsers are installed...");
        var exitCode = Microsoft.Playwright.Program.Main(new[] { "install", "chromium" });
        if (exitCode != 0)
        {
            Console.WriteLine("Warning: Browser installation may have issues, but continuing...");
        }

        using var playwright = await Playwright.CreateAsync();
        
        // Launch browser in headed mode
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
            SlowMo = 500
        });

        var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 720 }
        });

        var page = await context.NewPageAsync();

        // Demo 1: Web Search with Screenshots
        await DemoWebSearchWithScreenshots(page);
        
        // Demo 2: Yahoo Finance Stock Analysis
        await DemoYahooFinanceStockAnalysis(page);

        Console.WriteLine("\n=== Demo Complete! ===");
        Console.WriteLine("Check the 'screenshots' folder for captured images.");
        Console.WriteLine("These screenshots can be fed to an AI model for Computer Use automation.");
        Console.WriteLine("\nPress any key to close the browser...");
        Console.ReadKey();
    }

    static async Task DemoWebSearchWithScreenshots(IPage page)
    {
        Console.WriteLine("\n[Demo 1] Web search with screenshot capture...");
        
        // Create screenshots directory
        Directory.CreateDirectory("screenshots");
        
        // Step 1: Navigate
        await page.GotoAsync("https://www.bing.com");
        Console.WriteLine("✓ Navigated to Bing");
        await CaptureStep(page, "01_bing_homepage");

        // Step 2: Fill search box
        await page.FillAsync("textarea[name='q']", "Playwright Computer Use");
        Console.WriteLine("✓ Entered search query");
        await CaptureStep(page, "02_search_entered");

        // Step 3: Submit search
        await page.Keyboard.PressAsync("Enter");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        Console.WriteLine("✓ Search results loaded");
        await CaptureStep(page, "03_search_results");

        var title = await page.TitleAsync();
        Console.WriteLine($"✓ Page title: {title}");
    }

    static async Task DemoYahooFinanceStockAnalysis(IPage page)
    {
        Console.WriteLine("\n[Demo 2] Yahoo Finance Stock Analysis (MSFT)...");
        
        // Navigate to Yahoo Finance
        await page.GotoAsync("https://finance.yahoo.com");
        Console.WriteLine("✓ Navigated to Yahoo Finance");
        await CaptureStep(page, "04_yahoo_homepage");
        await Task.Delay(2000);

        // Search for MSFT
        try
        {
            // Try to find and fill the search box
            var searchSelectors = new[] { 
                "input[name='yfin-usr-qry']", 
                "input[type='text']",
                "#yfin-usr-qry",
                "[data-test='search-box']"
            };

            bool searchSucceeded = false;
            foreach (var selector in searchSelectors)
            {
                try
                {
                    var searchBox = await page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { Timeout = 3000 });
                    if (searchBox != null)
                    {
                        await searchBox.FillAsync("MSFT");
                        Console.WriteLine($"✓ Entered 'MSFT' in search box (selector: {selector})");
                        await CaptureStep(page, "05_search_entered");
                        
                        await page.Keyboard.PressAsync("Enter");
                        await Task.Delay(3000);
                        searchSucceeded = true;
                        break;
                    }
                }
                catch
                {
                    continue;
                }
            }

            if (!searchSucceeded)
            {
                // Alternative: Navigate directly to MSFT page
                Console.WriteLine("⚠ Search box not found, navigating directly to MSFT page");
                await page.GotoAsync("https://finance.yahoo.com/quote/MSFT");
                await Task.Delay(3000);
            }

            Console.WriteLine("✓ MSFT page loaded");
            await CaptureStep(page, "06_msft_quote_page");

            // Extract stock information
            await Task.Delay(2000); // Wait for dynamic content to load
            
            Console.WriteLine("\n📊 Analyzing MSFT Stock Data:");
            Console.WriteLine("=====================================");

            // Get all fin-streamer elements which contain live stock data
            var finStreamers = await page.QuerySelectorAllAsync("fin-streamer");
            
            var stockData = new Dictionary<string, string>();
            
            foreach (var streamer in finStreamers)
            {
                try
                {
                    var field = await streamer.GetAttributeAsync("data-field");
                    var value = await streamer.TextContentAsync();
                    
                    if (!string.IsNullOrWhiteSpace(field) && !string.IsNullOrWhiteSpace(value))
                    {
                        stockData[field] = value.Trim();
                    }
                }
                catch { }
            }

            // Display key metrics
            if (stockData.ContainsKey("regularMarketPrice"))
            {
                Console.WriteLine($"💰 Current Price: ${stockData["regularMarketPrice"]}");
            }
            
            if (stockData.ContainsKey("regularMarketChange"))
            {
                var change = stockData["regularMarketChange"];
                var changePercent = stockData.ContainsKey("regularMarketChangePercent") 
                    ? $" ({stockData["regularMarketChangePercent"]})" 
                    : "";
                var arrow = change.StartsWith("-") ? "📉" : "📈";
                Console.WriteLine($"{arrow} Change: {change}{changePercent}");
            }

            if (stockData.ContainsKey("regularMarketDayHigh"))
            {
                Console.WriteLine($"🔺 Day High: ${stockData["regularMarketDayHigh"]}");
            }
            
            if (stockData.ContainsKey("regularMarketDayLow"))
            {
                Console.WriteLine($"🔻 Day Low: ${stockData["regularMarketDayLow"]}");
            }

            if (stockData.ContainsKey("regularMarketOpen"))
            {
                Console.WriteLine($"🌅 Open: ${stockData["regularMarketOpen"]}");
            }

            if (stockData.ContainsKey("regularMarketPreviousClose"))
            {
                Console.WriteLine($"🌙 Previous Close: ${stockData["regularMarketPreviousClose"]}");
            }

            if (stockData.ContainsKey("regularMarketVolume"))
            {
                Console.WriteLine($"📊 Volume: {stockData["regularMarketVolume"]}");
            }

            // Get Market Cap and other stats from the page
            var statsElements = await page.QuerySelectorAllAsync("[data-test*='qsp']");
            foreach (var stat in statsElements)
            {
                try
                {
                    var testAttr = await stat.GetAttributeAsync("data-test");
                    var text = await stat.TextContentAsync();
                    
                    if (!string.IsNullOrWhiteSpace(testAttr) && !string.IsNullOrWhiteSpace(text))
                    {
                        if (testAttr.Contains("MARKET_CAP"))
                        {
                            Console.WriteLine($"🏢 Market Cap: {text.Trim()}");
                        }
                        else if (testAttr.Contains("PE_RATIO"))
                        {
                            Console.WriteLine($"📐 P/E Ratio: {text.Trim()}");
                        }
                    }
                }
                catch { }
            }

            // Get page title for context
            var title = await page.TitleAsync();
            Console.WriteLine($"\n📄 Page: {title}");

            // Analysis Summary
            if (stockData.ContainsKey("regularMarketPrice") && 
                stockData.ContainsKey("regularMarketOpen") &&
                stockData.ContainsKey("regularMarketPreviousClose"))
            {
                Console.WriteLine("\n💡 Today's Action Analysis:");
                
                try
                {
                    var currentPrice = decimal.Parse(stockData["regularMarketPrice"]);
                    var openPrice = decimal.Parse(stockData["regularMarketOpen"]);
                    var previousClose = decimal.Parse(stockData["regularMarketPreviousClose"]);
                    
                    var dayChange = currentPrice - openPrice;
                    var dayChangePercent = (dayChange / openPrice) * 100;
                    
                    Console.WriteLine($"   • Stock opened at ${openPrice:F2}");
                    Console.WriteLine($"   • Currently trading at ${currentPrice:F2}");
                    Console.WriteLine($"   • Intraday change: ${dayChange:F2} ({dayChangePercent:F2}%)");
                    
                    if (currentPrice > openPrice)
                    {
                        Console.WriteLine($"   • Status: ✅ UP from open (bullish session)");
                    }
                    else if (currentPrice < openPrice)
                    {
                        Console.WriteLine($"   • Status: ⚠️ DOWN from open (bearish session)");
                    }
                    else
                    {
                        Console.WriteLine($"   • Status: ➡️ FLAT from open");
                    }
                    
                    if (currentPrice > previousClose)
                    {
                        Console.WriteLine($"   • Trend: 🟢 Above yesterday's close");
                    }
                    else
                    {
                        Console.WriteLine($"   • Trend: 🔴 Below yesterday's close");
                    }
                }
                catch
                {
                    Console.WriteLine("   • Unable to calculate detailed analysis");
                }
            }

            Console.WriteLine("=====================================");
            await CaptureStep(page, "07_msft_analysis_complete");
            Console.WriteLine("\n✓ Stock analysis complete");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Error during stock analysis: {ex.Message}");
            await CaptureStep(page, "error_screenshot");
        }
    }

    static async Task CaptureStep(IPage page, string stepName)
    {
        var screenshotPath = Path.Combine("screenshots", $"{stepName}.png");
        await page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = screenshotPath
        });
        Console.WriteLine($"   📸 Screenshot saved: {screenshotPath}");
        
        // In a real AI-powered Computer Use system, these screenshots would be:
        // 1. Sent to an AI model (like Claude with Computer Use capabilities)
        // 2. The AI analyzes the screenshot to understand the current state
        // 3. The AI decides the next action (click, type, scroll, etc.)
        // 4. The action is executed via Playwright
        // 5. A new screenshot is taken and the loop continues
    }
}
