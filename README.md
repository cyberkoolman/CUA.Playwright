# Computer Use Agent with Playwright

This project demonstrates how to combine **Playwright browser automation** with **AI-powered Computer Use** capabilities, bridging the gap between the static screenshot approach from your previous project and real, live browser interactions.

## What This Demo Does

### Current Implementation (Working)
✅ **Playwright Automation** - Real browser control with:
- Web navigation and search
- Form filling and interaction
- Screenshot capture at each step
- Visible browser automation (non-headless)

### Integration Concept (from Previous Project)
The previous project used:
- **Azure AI Projects** with Computer Use capabilities
- **Static screenshots** fed to AI models
- **Simulated actions** based on AI decisions

### Combined Approach (What We've Built)
This project shows how to combine both:

1. **Playwright** provides real browser control
2. **Screenshot capture** at each step creates a feedback loop
3. **AI Model** (when integrated) can analyze screenshots and decide actions
4. **Actions execute** in the real browser via Playwright
5. **New screenshots** confirm the action worked
6. Loop continues until task complete

## How It Works

```
┌─────────────────────────────────────────────────────────┐
│                    AI Computer Use Loop                  │
└─────────────────────────────────────────────────────────┘

1. Take Screenshot (Playwright)
         ↓
2. Send to AI Model (e.g., Claude with Computer Use)
         ↓
3. AI Analyzes and Decides Next Action
         ↓
4. Execute Action in Browser (Playwright)
         ↓
5. Capture New State (Screenshot)
         ↓
   (Loop back to step 2)
```

## Running the Demo

```powershell
dotnet run
```

The demo will:
1. Launch a visible Chrome browser
2. Perform a web search on Bing
3. Navigate to a form page
4. Fill form fields
5. Capture screenshots at each step (saved to `screenshots/` folder)

## Screenshots Captured

- `01_bing_homepage.png` - Initial state
- `02_search_entered.png` - After typing query
- `03_search_results.png` - After search completion
- `04_form_page.png` - Form page loaded
- `05_form_field1_filled.png` - First field filled
- `06_form_field2_filled.png` - Second field filled (if visible)

## Key Differences from Previous Project

| Previous Project (Static) | This Project (Live) |
|---------------------------|---------------------|
| Pre-captured screenshots | Real-time screenshots |
| Simulated browser state | Actual browser |
| Mock form filling | Real form interaction |
| Static file-based workflow | Dynamic browser automation |
| AI decides based on old images | AI can control live browser |

## Integration with AI Models

To integrate with an AI Computer Use model:

1. **After each action**, capture a screenshot
2. **Send screenshot** to AI model with context
3. **Receive AI decision** (click coordinates, text to type, etc.)
4. **Execute via Playwright**:
   ```csharp
   await page.Mouse.ClickAsync(x, y);  // For clicks
   await page.FillAsync(selector, text);  // For text input
   await page.Keyboard.PressAsync(key);  // For key presses
   ```
5. **Capture new screenshot** and repeat

## Example AI Integration (Pseudo-code)

```csharp
while (!taskComplete)
{
    // Capture current state
    byte[] screenshot = await CaptureScreenshot(page);
    
    // Send to AI model
    var aiResponse = await aiModel.AnalyzeAndDecide(screenshot, taskDescription);
    
    // Execute AI's decision
    switch (aiResponse.Action)
    {
        case "click":
            await page.Mouse.ClickAsync(aiResponse.X, aiResponse.Y);
            break;
        case "type":
            await page.Keyboard.TypeAsync(aiResponse.Text);
            break;
        case "navigate":
            await page.GotoAsync(aiResponse.Url);
            break;
    }
    
    // Check if task is complete
    taskComplete = aiResponse.IsComplete;
}
```

## Technologies Used

- **.NET 10.0** - Latest .NET framework
- **Microsoft.Playwright** - Browser automation
- **C#** - Programming language

## Future Enhancements

To fully integrate the AI capabilities from the previous project:

1. Add Azure AI Projects SDK
2. Implement Computer Use API calls
3. Create action executor that maps AI decisions to Playwright commands
4. Add error handling and retry logic
5. Implement task completion detection
6. Add multi-step task orchestration

## Benefits of This Approach

✅ **Real Browser Testing** - See exactly what the AI sees and does
✅ **Live Feedback** - Actions execute in real-time
✅ **Screenshot Evidence** - Every step is documented
✅ **Flexible Automation** - Can handle any web task
✅ **AI-Guided** - Let AI figure out complex interactions
✅ **Verifiable** - Screenshots confirm success at each step

## Code Structure

- `Program.cs` - Main entry point
- `DemoWebSearchWithScreenshots()` - Search demonstration
- `DemoFormFillingWithScreenshots()` - Form interaction
- `CaptureStep()` - Screenshot utility

## Notes

This demo intentionally uses a **simplified approach** to focus on the core concept of combining Playwright with Computer Use workflows. The previous project's Azure AI integration had API compatibility issues that would require updating to the latest SDK versions and API patterns.

The key insight is: **Playwright provides the hands, AI provides the brain.**
