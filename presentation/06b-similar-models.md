# Similar Models & Technologies

## Anthropic Claude Computer Use

**Claude 3.5 Sonnet with Computer Use**
- Similar vision + action capabilities
- Returns tool calls for mouse/keyboard
- API format: `computer_20241022` tool
- Currently in public beta

```python
# Anthropic equivalent
response = client.messages.create(
    model="claude-3-5-sonnet-20241022",
    tools=[{
        "type": "computer_20241022",
        "display_width_px": 1280,
        "display_height_px": 720
    }]
)
```

---

## OpenAI GPT-4 Vision (Legacy Approach)

**GPT-4V (Vision) - No Computer Use Tool**
- Can analyze screenshots
- Returns text descriptions only
- ❌ No action coordinates
- ❌ No built-in automation format

**What you get:**
- "I see a search button in the top right"
- ❌ NOT: `Click(567, 330)`

---

## Google Gemini with Vision

**Gemini 2.0 Flash**
- Multimodal vision capabilities
- No native computer use tool (yet)
- Experimental automation features in development
