# Code Deep Dive: The Feedback Loop

## Running the Visual Analysis Cycle

```csharp
private static async Task InvokeComputerUseAgentAsync(AIAgent agent)
{
    byte[] screenshot = await _page.ScreenshotAsync();
    
    for (int iteration = 1; iteration <= 15; iteration++)
    {
        // Send screenshot to AI for visual analysis
        var response = await agent.SendMessageAsync(
            text: "Continue with your task",
            imageBytes: screenshot
        );
        
        // Check if AI thinks task is complete
        if (response.Content.Any(c => c.Text?.Contains("complete") == true))
        {
            Console.WriteLine("✅ AI TASK COMPLETE!");
            break;
        }
        
        // Execute AI's decisions
        foreach (var action in response.ComputerActions)
        {
            screenshot = await ComputerUseUtil.ExecuteComputerActionAsync(
                action, 
                _page
            );
        }
    }
}
```
