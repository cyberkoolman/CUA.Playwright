# The Problem: Traditional Web Automation

## Traditional Approach (Brittle)

```csharp
// Breaks when UI changes
await page.ClickAsync("#search-button");
await page.FillAsync("input[name='q']", "MSFT");
await page.ClickAsync(".submit-search");
```

❌ **Issues:**
- Hard-coded CSS selectors
- Breaks on UI redesigns
- Requires DOM knowledge
- No adaptability

---

## What If AI Could Just... See?

🤔 **Question:** Can AI navigate a website like a human would - by looking at the screen and clicking what it sees?

✅ **Answer:** Yes! With Azure AI Computer Use + Playwright
