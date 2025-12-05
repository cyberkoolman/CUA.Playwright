# Critical Breakthrough: Getting Click Coordinates

## The Challenge

Preview API returns `ComputerCallAction` but coordinates weren't documented

## The Solution: Reflection

```csharp
// Access X and Y properties via reflection
var actionTypeObj = action.GetType();

var xProp = actionTypeObj.GetProperty("X", 
    BindingFlags.Public | BindingFlags.Instance);
var yProp = actionTypeObj.GetProperty("Y", 
    BindingFlags.Public | BindingFlags.Instance);

if (xProp != null && yProp != null)
{
    int x = Convert.ToInt32(xProp.GetValue(action));
    int y = Convert.ToInt32(yProp.GetValue(action));
    
    Console.WriteLine($"🖱️ AI wants to click at ({x}, {y})");
    await page.Mouse.ClickAsync(x, y);
}
```

---

**Discovery:** `InternalComputerActionClick` HAS public `X` and `Y` properties!
