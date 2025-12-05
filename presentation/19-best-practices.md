# Best Practices

## 1. Clear AI Instructions
```csharp
const string AgentInstructions = @"
    Your task is to:
    1. Navigate to Yahoo Finance (already loaded)
    2. Click on the search box
    3. Type 'MSFT' in the search box
    4. Click the search button
    5. Report: current price, change, high, low, volume
    
    Be precise with your clicks.
";
```

---

## 2. Appropriate Wait Times
```csharp
await Task.Delay(1500);  // Let page settle
await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
```

---

## 3. Error Handling
```csharp
try {
    await page.Mouse.ClickAsync(x, y);
} catch (Exception ex) {
    Console.WriteLine($"⚠️ Error: {ex.Message}");
    // Capture error screenshot for debugging
}
```

---

## 4. Logging & Debugging
- Log every AI decision
- Save screenshots at each iteration
- Track action success/failure
