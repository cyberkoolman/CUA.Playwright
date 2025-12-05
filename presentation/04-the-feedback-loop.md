# The Visual Analysis Feedback Loop

## Continuous See → Think → Act Cycle

```
┌─────────────────────────────────────────────────────┐
│  1. Screenshot → AI (computer-use-preview model)    │
│     "What do you see? What should you do next?"     │
│                                                     │
│  2. AI analyzes pixels and decides:                 │
│     - ComputerCallAction: Click at (X, Y)           │
│     - ComputerCallAction: Type "text"               │
│     - ComputerCallAction: Scroll                    │
│                                                     │
│  3. ComputerUseUtil executes in Playwright:         │
│     - page.Mouse.ClickAsync(x, y)                   │
│     - page.Keyboard.TypeAsync(text)                 │
│     - page.Mouse.WheelAsync(scrollY)                │
│                                                     │
│  4. New screenshot → back to step 1                 │
└─────────────────────────────────────────────────────┘
```

---

**Each iteration:** AI sees current state → decides next action → sees result
