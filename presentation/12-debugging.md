# Debugging: Understanding the Action Object

## Inspecting Available Properties

```csharp
Console.WriteLine($"🔍 DEBUG - Action object details:");
Console.WriteLine($"Type: {action.GetType().FullName}");

// List all properties
var props = action.GetType()
    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
    .Select(p => $"{p.Name}:{p.PropertyType.Name}");
Console.WriteLine($"Properties: {string.Join(", ", props)}");

// List all fields
var fields = action.GetType()
    .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
    .Select(f => $"{f.Name}:{f.FieldType.Name}");
Console.WriteLine($"Fields: {string.Join(", ", fields)}");
```

---

### Output Example
```
Type: OpenAI.Responses.InternalComputerActionClick
Properties: Button:ComputerCallActionMouseButton, X:Int32, Y:Int32, ...
Fields: <X>k__BackingField:Int32, <Y>k__BackingField:Int32
```

---

## Debugging Wasted Iterations

**Symptom**: AI clicking/typing multiple times

```
ITERATION 1: Click (518, 60)
ITERATION 2: Click (534, 58)  ← Why again?
ITERATION 3: Click (476, 58)  ← Still clicking!
ITERATION 4: Type 'MSFT'
```

**Root Cause**: Vague instructions - AI unsure if action succeeded

**Fix**: Add explicit "ONCE" and conditional logic
```csharp
STEP 1: If blank page → click search box ONCE
STEP 2: If just clicked → type ONCE
STEP 3: If typed → click search ONCE (DO NOT type again)
```

**Result**: 4 iterations instead of 5+

---

## Detecting Repeated Actions

Add action history tracking:
```csharp
var actionHistory = new List<string>();

if (action is InternalComputerActionClick click) {
    var actionKey = $"Click({click.X},{click.Y})";
    if (actionHistory.LastOrDefault()?.StartsWith("Click") == true) {
        Console.WriteLine("⚠️  WARNING: Multiple clicks in sequence");
    }
    actionHistory.Add(actionKey);
}
```
