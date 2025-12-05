# Code Deep Dive: ComputerUseUtil.cs

## Executing AI Decisions in Playwright

```csharp
internal static async Task<byte[]> ExecuteComputerActionAsync(
    ComputerCallAction action,
    IPage page)
{
    // Execute the AI's visual decision
    await ExecuteActionOnPage(action, page);
    
    // Wait for browser to settle
    await Task.Delay(1500);
    
    // Capture new screenshot for AI to analyze
    byte[] screenshot = await page.ScreenshotAsync();
    
    return screenshot;
}
```

---

**Pattern:** Execute → Wait → Screenshot → Return to AI
