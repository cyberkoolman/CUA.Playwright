# Code Deep Dive: Program.cs

## Setting Up the AI Agent

```csharp
// MUST use computer-use-preview model
const string deploymentName = "computer-use-preview";

AIProjectClient aiProjectClient = new(
    new Uri(endpoint), 
    new AzureCliCredential()
);

// Create agent with Computer Use Tool
AIAgent agent = await aiProjectClient.CreateAIAgentAsync(
    name: "FinanceAgent-MSFT",
    model: deploymentName,
    instructions: AgentInstructions,
    tools: [
        ResponseTool.CreateComputerTool(
            ComputerToolEnvironment.Browser, 
            width: 1280, 
            height: 720
        ).AsAITool()
    ]
);
```

---

**Key:** The `ResponseTool.CreateComputerTool()` enables visual analysis capabilities
