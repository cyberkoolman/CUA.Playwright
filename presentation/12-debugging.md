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
