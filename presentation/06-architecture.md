# Architecture Overview

## Three Main Components

### 1. Azure AI Agent (computer-use-preview model)
- Receives screenshots as input
- Returns `ComputerCallAction` decisions
- No DOM knowledge - pure visual analysis

### 2. Program.cs - The Orchestrator
- Creates AI agent with Computer Use Tool
- Runs the feedback loop
- Sends screenshots to AI
- Receives action decisions

### 3. ComputerUseUtil.cs - The Executor
- Bridges AI decisions to Playwright
- Translates `ComputerCallAction` → browser actions
- Captures new screenshots for AI

---

```
Azure AI ←→ Program.cs ←→ ComputerUseUtil ←→ Playwright Browser
(Vision)    (Orchestrate)   (Execute)         (Real Web)
```
