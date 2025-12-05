# AI Action Types

## What the AI Can Decide to Do

| AI Decision | ComputerCallAction Type | Playwright Execution |
|-------------|------------------------|---------------------|
| **Click** | `InternalComputerActionClick` | `page.Mouse.ClickAsync(x, y)` |
| **Type** | `InternalComputerActionTypeKeys` | `page.Keyboard.TypeAsync(text)` |
| **Press Key** | `InternalComputerActionPressKeys` | `page.Keyboard.PressAsync(key)` |
| **Scroll** | `InternalComputerActionScroll` | `page.Mouse.WheelAsync(deltaY)` |

---

### Example: Type Action
```csharp
if (action.Kind == ComputerCallActionKind.Type && action.TypeText != null)
{
    Console.WriteLine($"💬 AI wants to type: '{action.TypeText}'");
    await page.Keyboard.TypeAsync(action.TypeText);
}
```
