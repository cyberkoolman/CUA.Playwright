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
