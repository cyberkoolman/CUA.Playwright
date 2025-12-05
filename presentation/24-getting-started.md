# Getting Started for Your Team

## Step 1: Clone the Repository
```bash
git clone https://github.com/cyberkoolman/CUA.Playwright.git
cd CUA.Playwright/CUA.Playwright
```

---

## Step 2: Set Up Azure AI
1. Create Azure AI Foundry project
2. Deploy `computer-use-preview` model
3. Copy project endpoint URL

---

## Step 3: Configure Environment
```powershell
$env:AZURE_FOUNDRY_PROJECT_ENDPOINT = "https://your-project.azureai.azure.com"
```

---

## Step 4: Install Dependencies
```bash
dotnet restore
dotnet build
```

---

## Step 5: Run!
```bash
dotnet run
```

---

## Step 6: Customize
- Modify `AgentInstructions` for different tasks
- Change target URL in `Program.cs`
- Extend action types in `ComputerUseUtil.cs`
