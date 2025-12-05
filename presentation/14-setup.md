# Setup & Configuration

## Prerequisites

1. **.NET 10.0 SDK**
2. **Azure AI Foundry project** with Computer Use enabled
3. **Playwright** (auto-installs browsers)

---

## Required Environment Variables

```powershell
# Your Azure AI project endpoint
$env:AZURE_FOUNDRY_PROJECT_ENDPOINT = "https://your-project.azureai.azure.com"
```

---

## Model Configuration

The model is **hardcoded** to `computer-use-preview`:

```csharp
const string deploymentName = "computer-use-preview";
```

**Why?** Only this model supports Computer Use Tool API

---

## Running the Demo

```bash
dotnet run
```

If Azure not configured → falls back to basic Playwright demo
