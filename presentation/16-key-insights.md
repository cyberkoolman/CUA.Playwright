# Key Technical Insights

## 1. Model Selection is Critical
- ✅ `computer-use-preview` - Only model with Computer Use Tool
- ❌ `gpt-4o`, `gpt-4o-2024-11-20` - Don't support Computer Use

---

## 2. Coordinates ARE Accessible
- `InternalComputerActionClick` has public `X` and `Y` properties
- Access via reflection or direct property access
- No need to parse strings or guess

---

## 3. Visual-Only Analysis
- AI sees **pixels only**, not HTML/DOM
- Works like a human viewing the screen
- More resilient to UI changes

---

## 4. Iterative Feedback Loop
- Each action creates new state
- AI re-analyzes after every action
- Self-correcting behavior

---

## 5. Instruction Precision Matters
**Problem**: Vague instructions cause wasted iterations
- ❌ "Click the search box" → AI clicks 3 times
- ✅ "Click the search box ONE TIME" → AI clicks once

**Solution**: Conditional step-based instructions
```
STEP 1: If blank page → click search box ONCE
STEP 2: If clicked → type 'MSFT' ONCE
STEP 3: If typed → click search button ONCE
STEP 4: If results → STOP actions, just READ
```

---

## 6. Search Engine Selection
| Engine | CAPTCHA | Layout | Best For |
|--------|---------|--------|----------|
| Yahoo Finance | ❌ | Finance-focused | Stock data only |
| Bing | ❌ | Complex | General search |
| Google | ✅ **BLOCKS** | Complex | ❌ Can't automate |
| DuckDuckGo | ❌ | Simple | ✅ **Best choice** |

**Critical**: Google shows CAPTCHA for automation - AI cannot solve it

---

## 7. Keyboard Input Unreliable
- Enter key doesn't consistently submit forms
- **Solution**: Disable keyboard, use click-only
- Find and click submit buttons instead

---

## 8. AI Action Prioritization
**Default behavior**: AI prioritizes gathering info over analyzing
- Sees stock price → scrolls for more data
- Should: Read visible data → report → then scroll

**Fix**: Explicit "DO NOT SCROLL - read first" instructions
