# AI-Powered Computer Use Agent with Playwright

This project demonstrates **true AI Computer Use** by combining Azure AI's `computer-use-preview` model with Playwright browser automation. The AI **visually analyzes screenshots** and decides what to click, type, or scroll - then Playwright executes those actions in a real browser.

## 🎯 What This Does

Watch the AI navigate DuckDuckGo **by vision alone**:

```
1. AI sees screenshot of DuckDuckGo homepage
2. AI decides: "I should click the search box at coordinates (466, 60)"
3. Playwright clicks at those exact coordinates
4. AI sees new screenshot showing search box is focused
5. AI decides: "I should type 'MSFT'"
6. Playwright types into the search box
7. AI sees screenshot with "MSFT" typed
8. AI decides: "I should click the search button at (916, 59)"
9. Playwright clicks the search button
10. AI sees results page and reports: "Current Price: $483.16, Change: +$2.32 (+0.48%)"
```

The AI doesn't know HTML, CSS selectors, or DOM structure - it **only sees pixels** like a human would.

## 🔄 The Visual Analysis Loop

**How It Works:**
```
┌─────────────────────────────────────────────────────┐
│  1. Screenshot → AI (Azure computer-use-preview)    │
│     "What do you see? What should you do next?"     │
│                                                     │
│  2. AI analyzes pixels and decides:                 │
│     - ComputerCallAction: Click at (X, Y)           │
│     - ComputerCallAction: Type "text"               │
│     - ComputerCallAction: Scroll                    │
│                                                     │
│  3. ComputerUseUtil executes in Playwright:         │
│     - page.Mouse.ClickAsync(x, y)                   │
│     - page.Keyboard.TypeAsync(text)                 │
│     - page.Mouse.WheelAsync(scrollY)                │
│                                                     │
│  4. New screenshot → back to step 1                 │
└─────────────────────────────────────────────────────┘
```

## 🔧 Technical Implementation

### Key Breakthrough: Accessing Click Coordinates

The preview API returns `InternalComputerActionClick` objects with **public `X` and `Y` properties**:

```csharp
// From ComputerUseUtil.cs - The critical discovery
var actionTypeObj = action.GetType();
var xProp = actionTypeObj.GetProperty("X", BindingFlags.Public | BindingFlags.Instance);
var yProp = actionTypeObj.GetProperty("Y", BindingFlags.Public | BindingFlags.Instance);

if (xProp != null && yProp != null)
{
    int x = Convert.ToInt32(xProp.GetValue(action));
    int y = Convert.ToInt32(yProp.GetValue(action));
    await page.Mouse.ClickAsync(x, y);  // Execute AI's decision
}
```

### Action Types Handled

| AI Decision | ComputerCallAction Type | Playwright Execution |
|-------------|------------------------|---------------------|
| Click button | `InternalComputerActionClick` | `page.Mouse.ClickAsync(x, y)` |
| Type text | `InternalComputerActionTypeKeys` | `page.Keyboard.TypeAsync(text)` |
| Press key | `InternalComputerActionPressKeys` | `page.Keyboard.PressAsync(key)` |
| Scroll | `InternalComputerActionScroll` | `page.Mouse.WheelAsync(deltaY)` |

### Architecture

**Program.cs** - AI feedback loop:
```csharp
// Create AI agent with Computer Use tool
AIAgent agent = await aiProjectClient.CreateAIAgentAsync(
    model: "computer-use-preview",  // MUST use this model
    tools: [ResponseTool.CreateComputerTool(ComputerToolEnvironment.Browser, 1280, 720)]
);

// Visual analysis loop
while (!done) {
    screenshot = await page.ScreenshotAsync();
    response = await agent.SendMessageAsync(screenshot);
    
    foreach (var action in response.ComputerActions) {
        screenshot = await ComputerUseUtil.ExecuteComputerActionAsync(action, page);
    }
}
```

**ComputerUseUtil.cs** - Bridges AI → Playwright:
```csharp
internal static async Task<byte[]> ExecuteComputerActionAsync(
    ComputerCallAction action,
    IPage page)
{
    // Execute AI's visual decision in real browser
    await ExecuteActionOnPage(action, page);
    
    // Capture result for AI to see
    return await page.ScreenshotAsync();
}
```

## ⚙️ Setup & Run

### Prerequisites
1. **Azure AI Foundry project** with `computer-use-preview` model deployed
2. **.NET 10.0 SDK**
3. **Playwright browsers** (auto-installs on first run)

### Environment Variables
```powershell
# Required - your Azure AI project endpoint
$env:AZURE_FOUNDRY_PROJECT_ENDPOINT = "https://your-project.azureai.azure.com"
```

The model is hardcoded to `computer-use-preview` - the only model that supports Computer Use Tool.

### Run
```bash
dotnet run
```

**What you'll see:**
```
🤖 AI DECIDED ACTION: Click
   🖱️  AI wants to click at (466, 60)  [DuckDuckGo search box]

🤖 AI DECIDED ACTION: Type
   💬 AI wants to type: 'MSFT'

🤖 AI DECIDED ACTION: Click  
   🖱️  AI wants to click at (916, 59)  [search button]

✅ AI TASK COMPLETE!
   "Current Price: $483.16
    Day Change: +$2.32 (+0.48%)
    Open: $482.515, High: $483.40, Low: $478.88
    Volume: 26.2M, Market Cap: 3.59T"
```

## 🆚 Comparison: Static vs Live

| Aspect | Microsoft Sample | This Implementation |
|--------|-----------------|-------------------|
| **Browser** | Simulated (PNG images) | ✅ Real Playwright automation |
| **Screenshots** | Pre-captured files | ✅ Live `page.ScreenshotAsync()` |
| **Clicks** | Mock coordinates | ✅ Actual `page.Mouse.ClickAsync(x, y)` |
| **Data** | Static demo | ✅ Live DuckDuckGo search results |
| **Navigation** | Fixed sequence | ✅ Real web interactions |
| **CAPTCHA** | N/A | ✅ Avoided (DuckDuckGo > Google) |

Both use the **same AI visual analysis**, but this executes in a **real browser**.

## 📊 Example Run Output

```
=== AI-Powered Computer Use Agent with Playwright ===

✓ Browser ready at DuckDuckGo

🔄 ITERATION 1 - AI Visual Analysis Cycle
🤖 AI DECIDED ACTION: Click
   🖱️  AI wants to click at (466, 60)  [search box]

🔄 ITERATION 2 - AI Visual Analysis Cycle  
🤖 AI DECIDED ACTION: Type
   💬 AI wants to type: 'MSFT'

🔄 ITERATION 3 - AI Visual Analysis Cycle
🤖 AI DECIDED ACTION: Click
   🖱️  AI wants to click at (916, 59)  [search button]

🔄 ITERATION 4 - AI Visual Analysis Cycle
👁️  AI SEES & THINKS:
   "Current Price: $483.16, Change: +$2.32 (+0.48%)
    Open: $482.515, High: $483.40, Low: $478.88
    Volume: 26.2M, Market Cap: 3.59T
    TASK COMPLETE"

✅ AI TASK COMPLETE!
   Total visual analysis cycles: 4
```

**Optimization Journey:**
- Initial implementation: 12+ iterations (multiple redundant clicks)
- After instruction optimization: **4 iterations** (click → type → click → read)

## 🔑 Key Insights

1. **Model matters**: Only `computer-use-preview` supports Computer Use Tool
2. **Coordinates are public**: `InternalComputerActionClick` has `X` and `Y` properties accessible via reflection
3. **Visual-only**: AI sees screenshots, not HTML/DOM
4. **Iterative**: Each action → new screenshot → AI re-analyzes
5. **Instruction clarity is critical**: Vague instructions cause wasted iterations
   - ❌ "Click the search box" → AI clicks 3 times at different coordinates
   - ✅ "Click the search box ONE TIME" → AI clicks once
6. **Conditional instructions work better**: "If you see X, do Y" prevents repeated actions
7. **Search engine selection matters**:
   - Yahoo Finance: Works but limited to finance data
   - Bing: Works but AI struggled with layout
   - Google: **CAPTCHA blocks automation** - cannot be solved by AI
   - DuckDuckGo: ✅ Simple layout, no CAPTCHA, best for automation
8. **Keyboard input unreliable**: Enter key doesn't consistently trigger form submission
   - Disable keyboard shortcuts, use click-only interaction
9. **AI prioritizes actions over analysis**: Without explicit instructions, AI scrolls before reading
   - Must explicitly say "DO NOT SCROLL" and "READ first"

## 🔗 References

- [Microsoft agent-framework Computer Use Sample](https://github.com/microsoft/agent-framework/tree/main/dotnet/samples/GettingStarted/FoundryAgents/FoundryAgents_Step15_ComputerUse)
- [Azure AI Computer Use Documentation](https://learn.microsoft.com/en-us/azure/ai-services/agents/computer-use)

---

**Status**: ✅ Fully functional - AI successfully navigates and analyzes web pages by vision alone!
