// Copyright (c) Microsoft. All rights reserved.
// This sample shows how to use Computer Use Tool with AI Agents + Playwright.

#pragma warning disable OPENAI001, OPENAICUA001

using Azure.AI.Projects;
using Azure.AI.Projects.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Responses;
using PlaywrightLib = Microsoft.Playwright;

namespace CUA.Playwright;

internal sealed class Program
{
    private static PlaywrightLib.IPage? _page;

    private static async Task Main(string[] args)
    {
        Console.WriteLine("=== AI-Powered Computer Use Agent with Playwright ===\n");

        string? endpoint = Environment.GetEnvironmentVariable("AZURE_FOUNDRY_PROJECT_ENDPOINT");
        if (string.IsNullOrEmpty(endpoint))
        {
            Console.WriteLine("ERROR: AZURE_FOUNDRY_PROJECT_ENDPOINT environment variable is not set.");
            Console.WriteLine("\nTo use the AI-powered Computer Use features, you need:");
            Console.WriteLine("1. An Azure AI Foundry project");
            Console.WriteLine("2. A deployed computer-use-preview model");
            Console.WriteLine("3. Set environment variable: AZURE_FOUNDRY_PROJECT_ENDPOINT");
            Console.WriteLine("\nFor now, running basic Playwright demo without AI...\n");
            await RunBasicPlaywrightDemo();
            return;
        }
        
        // MUST use computer-use-preview model for Computer Use Tool support
        const string deploymentName = "computer-use-preview";

        // Initialize Playwright
        Console.WriteLine("Initializing Playwright browser...");
        using var playwright = await PlaywrightLib.Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new PlaywrightLib.BrowserTypeLaunchOptions
        {
            Headless = false,
            SlowMo = 500
        });

        var context = await browser.NewContextAsync(new PlaywrightLib.BrowserNewContextOptions
        {
            ViewportSize = new PlaywrightLib.ViewportSize { Width = 1280, Height = 720 }
        });

        _page = await context.NewPageAsync();

        // Navigate to starting page with longer timeout
        await _page.GotoAsync("https://www.duckduckgo.com", new PlaywrightLib.PageGotoOptions { Timeout = 60000 });
        await Task.Delay(2000);
        Console.WriteLine("✓ Browser ready at DuckDuckGo\n");

        // Get a client to create/retrieve/delete server side agents with Azure Foundry Agents
        AIProjectClient aiProjectClient = new(new Uri(endpoint), new AzureCliCredential());

        const string AgentInstructions = @"
            You are a web search assistant. Follow these steps EXACTLY in order:
            
            STEP 1: If you see a blank DuckDuckGo search page:
                    - Click the search box ONCE
            
            STEP 2: If you just clicked the search box:
                    - Type 'MSFT' ONCE (do not type again if already typed)
            
            STEP 3: If you typed 'MSFT' and see the search button:
                    - Click the search button (magnifying glass icon) ONCE
                    - DO NOT type again after clicking search button
            
            STEP 4: If you see search RESULTS (prices, charts, links):
                    - STOP taking actions
                    - READ the information at the TOP of the page
                    - Report these details:
                      * Current Price: $XXX.XX
                      * Day Change: +$X.XX (+X.XX%)
                      * Open, High, Low, Volume, Market Cap
                      * Any visible news
                    - Say 'TASK COMPLETE'
            
            CRITICAL RULES: 
            - DO NOT type 'MSFT' more than once
            - DO NOT click multiple times on same element
            - Once search results appear, STOP clicking/typing and just READ
            - NEVER use Enter key
            - DO NOT SCROLL
        ";

        const string AgentName = "DuckDuckGoSearchAgent-MSFT";

        // Create AIAgent with Computer Use Tool
        AIAgent agent = await aiProjectClient.CreateAIAgentAsync(
            name: AgentName,
            model: deploymentName,
            instructions: AgentInstructions,
            description: "AI agent that can analyze stocks using real browser automation.",
            tools: [
                ResponseTool.CreateComputerTool(ComputerToolEnvironment.Browser, 1280, 720).AsAITool(),
            ]);

        try
        {
            await InvokeComputerUseAgentAsync(agent);
        }
        finally
        {
            await aiProjectClient.Agents.DeleteAgentAsync(agent.Name);
            Console.WriteLine($"\n✓ Agent '{AgentName}' cleaned up");
        }

        Console.WriteLine("\nPress any key to close browser...");
        Console.ReadKey();
    }

    private static async Task InvokeComputerUseAgentAsync(AIAgent agent)
    {
        if (_page == null) throw new InvalidOperationException("Browser page not initialized");

        // Take initial screenshot
        byte[] initialScreenshot = await _page.ScreenshotAsync();

        ChatOptions chatOptions = new();
        ResponseCreationOptions responseCreationOptions = new()
        {
            TruncationMode = ResponseTruncationMode.Auto
        };
        chatOptions.RawRepresentationFactory = (_) => responseCreationOptions;
        ChatClientAgentRunOptions runOptions = new(chatOptions)
        {
            AllowBackgroundResponses = true,
        };

        AgentThread thread = agent.GetNewThread();

        ChatMessage message = new(ChatRole.User, [
            new TextContent("Search for 'MSFT' on DuckDuckGo. Click search box, type MSFT, click search button. Once results load, STOP and READ the TOP of the page (DO NOT SCROLL). Report the stock information visible at the top. Say 'TASK COMPLETE' when done."),
            new DataContent(new BinaryData(initialScreenshot), "image/png")
        ]);

        // Initial request with screenshot - AI will LOOK at the page and decide what to do
        Console.WriteLine("🤖 AI Agent analyzing screenshot...");
        Console.WriteLine("    The AI is LOOKING at the DuckDuckGo page");
        Console.WriteLine("    It will decide what to click/type based on what it SEES\n");
        AgentRunResponse runResponse = await agent.RunAsync(message, thread: thread, options: runOptions);

        // Main interaction loop
        const int MaxIterations = 25;  // Increased for search + comprehensive analysis
        int iteration = 0;
        string initialCallId = string.Empty;

        Console.WriteLine("\n" + new string('=', 70));
        Console.WriteLine("   AI AGENT FEEDBACK LOOP - Computer Use in Action");
        Console.WriteLine(new string('=', 70));
        Console.WriteLine("The AI will:");
        Console.WriteLine("  1. LOOK at screenshot (visual analysis)");
        Console.WriteLine("  2. DECIDE what action to take");
        Console.WriteLine("  3. EXECUTE action in browser");
        Console.WriteLine("  4. SEE result → repeat\n");

        while (true)
        {
            // Poll until the response is complete
            while (runResponse.ContinuationToken is { } token)
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
                runOptions.ContinuationToken = token;
                runResponse = await agent.RunAsync(thread, runOptions);
            }

            if (iteration >= MaxIterations)
            {
                Console.WriteLine($"\n⚠️ Reached maximum iterations ({MaxIterations}). Stopping.");
                break;
            }

            iteration++;
            Console.WriteLine($"\n{new string('─', 70)}");
            Console.WriteLine($"🔄 ITERATION {iteration} - AI Visual Analysis Cycle");
            Console.WriteLine(new string('─', 70));

            // Display agent's thoughts
            var textContents = runResponse.Messages
                .SelectMany(m => m.Contents)
                .OfType<TextContent>();

            bool taskComplete = false;
            foreach (var textContent in textContents)
            {
                if (!string.IsNullOrWhiteSpace(textContent.Text))
                {
                    Console.WriteLine($"\n👁️  AI SEES & THINKS:");
                    Console.WriteLine($"    \"{textContent.Text}\"");
                    
                    // Check if AI detected CAPTCHA
                    if (textContent.Text.Contains("CAPTCHA", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine($"\n{new string('=', 70)}");
                        Console.WriteLine("⚠️  CAPTCHA DETECTED!");
                        Console.WriteLine("   AI cannot solve CAPTCHAs - manual intervention required");
                        Console.WriteLine("   Please solve the CAPTCHA manually and restart the demo");
                        Console.WriteLine(new string('=', 70));
                        taskComplete = true;
                    }
                    
                    // Check if AI says it's done
                    if (textContent.Text.Contains("TASK COMPLETE", StringComparison.OrdinalIgnoreCase) ||
                        textContent.Text.Contains("task is complete", StringComparison.OrdinalIgnoreCase))
                    {
                        taskComplete = true;
                    }
                }
            }

            // Check for computer actions
            IEnumerable<ComputerCallResponseItem> computerCalls = runResponse.Messages
                .SelectMany(x => x.Contents)
                .Where(c => c.RawRepresentation is ComputerCallResponseItem and not null)
                .Select(c => (ComputerCallResponseItem)c.RawRepresentation!);

            ComputerCallResponseItem? firstCall = computerCalls.FirstOrDefault();
            
            // If AI said task is complete OR no more actions, we're done
            if (taskComplete || firstCall is null)
            {
                Console.WriteLine($"\n{new string('=', 70)}");
                Console.WriteLine("✅ AI TASK COMPLETE!");
                Console.WriteLine($"   Total visual analysis cycles: {iteration}");
                Console.WriteLine($"   The AI successfully navigated and analyzed the page by SEEING it");
                Console.WriteLine(new string('=', 70));
                break;
            }

            // Process the action
            ComputerCallAction action = firstCall.Action;
            string currentCallId = firstCall.CallId;

            if (string.IsNullOrEmpty(initialCallId))
            {
                initialCallId = currentCallId;
            }

            Console.WriteLine($"\n🤖 AI DECIDED ACTION: {action.Kind}");
            if (action.TypeText != null)
            {
                Console.WriteLine($"    AI saw a text field and wants to type: '{action.TypeText}'");
            }

            // Execute the AI's decision in the REAL browser
            Console.WriteLine($"\n⚙️  EXECUTING in Playwright browser...");
            byte[] newScreenshot = await ComputerUseUtil.ExecuteComputerActionAsync(action, _page);
            
            Console.WriteLine($"📸 Sending new screenshot back to AI for analysis...");

            // Send result back to agent
            AIContent content = new()
            {
                RawRepresentation = new ComputerCallOutputResponseItem(
                    initialCallId,
                    output: ComputerCallOutput.CreateScreenshotOutput(new BinaryData(newScreenshot), "image/png"))
            };

            message = new(ChatRole.User, [content]);
            runResponse = await agent.RunAsync(message, thread: thread, options: runOptions);
        }
    }

    // Fallback demo if Azure AI is not configured
    private static async Task RunBasicPlaywrightDemo()
    {
        using var playwright = await PlaywrightLib.Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new PlaywrightLib.BrowserTypeLaunchOptions
        {
            Headless = false,
            SlowMo = 500
        });

        var context = await browser.NewContextAsync(new PlaywrightLib.BrowserNewContextOptions
        {
            ViewportSize = new PlaywrightLib.ViewportSize { Width = 1280, Height = 720 }
        });

        var page = await context.NewPageAsync();
        
        Console.WriteLine("Running basic Playwright automation...");
        await page.GotoAsync("https://www.duckduckgo.com");
        await Task.Delay(2000);
        
        // Search for MSFT
        var searchBox = await page.QuerySelectorAsync("input[type='text']");
        if (searchBox != null)
        {
            await searchBox.FillAsync("MSFT");
            await page.Keyboard.PressAsync("Enter");
            await Task.Delay(3000);
            Console.WriteLine("✓ Navigated to MSFT stock page");
        }
        
        Console.WriteLine("\nPress any key to close...");
        Console.ReadKey();
    }
}
