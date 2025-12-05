# Why We Use `computer-use-preview`

## Advantages for .NET Development

### 1. **Native Azure Integration**
```csharp
AIProjectClient aiProjectClient = new(
    new Uri(endpoint), 
    new AzureCliCredential()  // Seamless auth
);
```
- Works with existing Azure credentials
- Integrated with Azure AI Foundry
- Enterprise-ready security

---

### 2. **Structured Action Objects**
```csharp
ComputerCallAction action = response.ComputerActions.First();
int x = action.X;  // Strongly typed
int y = action.Y;
```
- Type-safe C# objects
- No JSON parsing needed
- IntelliSense support

---

### 3. **Microsoft Ecosystem**
- Official Microsoft.Agents.AI.AzureAI package
- Documented in Microsoft Learn
- Supported by Azure AI team
- Sample code provided

---

### 4. **Preview Access**
- Early access to latest capabilities
- Direct feedback channel to Azure AI team
- Production-ready despite "preview" label
