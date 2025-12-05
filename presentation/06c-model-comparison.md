# Model Comparison Table

## Computer Use Capabilities

| Model | Provider | Vision | Action Output | Coordinates | Status |
|-------|----------|--------|---------------|-------------|--------|
| **computer-use-preview** | Azure AI | ✅ Yes | `ComputerCallAction` | ✅ X, Y | **Production** |
| **Claude 3.5 Sonnet** | Anthropic | ✅ Yes | Tool calls | ✅ X, Y | Beta |
| **GPT-4o** | OpenAI | ✅ Yes | Text only | ❌ No | N/A |
| **GPT-4 Vision** | OpenAI | ✅ Yes | Text only | ❌ No | Legacy |
| **Gemini 2.0** | Google | ✅ Yes | Text only | ❌ No | Experimental |

---

## Action Format Comparison

**Azure computer-use-preview:**
```csharp
ComputerCallAction {
    Kind: Click,
    X: 567,
    Y: 330
}
```

**Anthropic Claude:**
```json
{
    "type": "computer_20241022",
    "action": "mouse_move",
    "coordinate": [567, 330]
}
```

**GPT-4 Vision:**
```
"Click the search button located at approximately x=567, y=330"
```
*(Requires parsing)*
