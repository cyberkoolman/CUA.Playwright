# Current Limitations & Considerations

## Performance
- ⏱️ Each iteration requires AI API call
- ⏱️ Screenshot encoding/decoding overhead
- ⏱️ ~1-3 seconds per action

**Tradeoff:** Adaptability vs Speed

---

## Accuracy
- 🎯 AI might misidentify UI elements
- 🎯 Coordinate precision varies
- 🎯 May require multiple attempts

**Mitigation:** Clear instructions, error handling

---

## Cost
- 💰 API calls per action
- 💰 Vision model inference costs
- 💰 Screenshot processing

**Consideration:** Balance automation value vs API costs

---

## Iteration Limit
- 🔢 Set to 15 iterations by default
- 🔢 Prevents infinite loops
- 🔢 May need adjustment per task
