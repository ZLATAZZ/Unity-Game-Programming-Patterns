# Unity Game Programming Patterns

A growing collection of small Unity projects focused on implementing and exploring common game programming patterns in C#.

The repository is based on concepts from Robert Nystrom's *Game Programming Patterns*, but each example is implemented as an independent practical exercise rather than a direct copy of the book's code.

The goal is to practice not only the patterns themselves, but also clean architecture, explicit dependencies, maintainable C# code, testing, and Unity-specific design decisions.

## Patterns

### [01 — Command Pattern](./01_CommandPattern_CarConfigurator/)

A car configurator demonstrating commands, undo/redo history, and reversible state changes.

### [02 — Flyweight Pattern](./02_FlyweightPattern_ArcaneDeck/)

A card-based example separating shared intrinsic data from per-instance runtime state.

### [03 — Observer Pattern](./03_ObserverPattern_BloodMoon/)

A Blood Moon gameplay scenario where several independent systems react to the same state transition through explicit observers.

### [04 — Prototype Pattern](./04_Prototype_Forge/)

A data-driven monster prototype demo using deep cloning, a generic prototype registry, JSON configuration, typed identifiers, and Edit Mode tests.