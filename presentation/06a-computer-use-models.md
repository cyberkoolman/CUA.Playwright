# Computer Use Models Explained

## What is `computer-use-preview`?

**Azure AI's Computer Use Model**
- Specialized AI model with **vision + action** capabilities
- Analyzes screenshots and returns executable actions
- Part of Azure AI Foundry (formerly Azure AI Studio)
- Based on OpenAI's computer use research

---

## How It Differs from Regular LLMs

| Feature | Standard GPT-4o | computer-use-preview |
|---------|----------------|---------------------|
| **Input** | Text only | Text + Screenshots |
| **Output** | Text response | `ComputerCallAction` objects |
| **Vision** | No native vision | ✅ Visual analysis |
| **Actions** | Describes actions | ✅ Returns executable commands |
| **Coordinates** | N/A | ✅ Returns X, Y positions |
| **Purpose** | Conversation | ✅ Computer automation |