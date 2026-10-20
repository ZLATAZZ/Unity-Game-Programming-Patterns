# Unity Game Programming Patterns

**From design pattern theory to working Unity systems.**

A growing collection of independent Unity projects exploring game programming patterns in C#, inspired by Robert Nystrom's *Game Programming Patterns*.

Rather than directly reproducing the book's examples, each project applies a pattern to a practical gameplay scenario while exploring the architectural decisions behind it.

The focus is not just on **making a pattern work**, but on understanding **when it is useful, how it affects maintainability, and what trade-offs it introduces**.

## Featured Project - Cursed Forest

**State Pattern | Data-Driven NPC AI | Generic FSM | Zenject | 32 Edit Mode Tests**

What happens when a Monster attacks a Villager—and a Knight responds?

Cursed Forest explores this interaction through an extensible NPC AI system featuring:

- Three NPC roles with distinct behaviors
- A reusable, generic FSM core
- ScriptableObject-driven states and transitions
- Composable transition conditions
- Modular movement, perception, combat, and energy systems
- Automated tests for the FSM infrastructure

Unlike a basic FSM demonstration, this project examines how the State Pattern can support multiple NPC types while keeping core logic separate from Unity-specific behavior.

**[Explore Cursed Forest ->](./06_StatePattern_ReusableNPCAI/)**

## Patterns

### [01 - Command Pattern](./01_CommandPattern_CarConfigurator/)

A car configurator demonstrating commands, undo/redo history, and reversible state changes.

### [02 - Flyweight Pattern](./02_FlyweightPattern_ArcaneDeck/)

A card-based example separating shared intrinsic data from per-instance runtime state.

### [03 - Observer Pattern](./03_ObserverPattern_BloodMoon/)

A Blood Moon gameplay scenario where several independent systems react to the same state transition through explicit observers.

### [04 - Prototype Pattern](./04_Prototype_Forge/)

A data-driven monster prototype demo using deep cloning, a generic prototype registry, JSON configuration, typed identifiers, and Edit Mode tests.

### [05 - Singleton Pattern](./05_SingletonPattern_PersistentRunSession/)

A persistent game-session demo across two Unity scenes, demonstrating instance management, global access, and shared state.

**Architectural perspective:** This example intentionally uses Singleton to demonstrate both its convenience and its limitations, particularly hidden dependencies and global mutable state.

### [06 - State Pattern](./06_StatePattern_ReusableNPCAI/)

A reusable NPC AI architecture featuring a generic FSM core, data-driven state and transition definitions, composable conditions, dependency injection, and three interacting NPC types.

**Architectural perspective:** Explores the balance between a simple state machine and a more extensible AI system, including the additional complexity introduced by abstraction and configuration.

## Engineering Focus

Across the projects, I aim to practice:

- **Clean C# design:** explicit responsibilities, interfaces, and modular components.
- **Separation of concerns:** keeping reusable mechanisms independent of concrete gameplay.
- **Testability:** validating behavior with automated tests where appropriate.
- **Extensibility:** adding new functionality while minimizing changes to existing components.
- **Architectural trade-offs:** recognizing when a pattern simplifies a problem—and when it introduces unnecessary complexity.

## Philosophy

**A design pattern is a tool, not a goal.**

Each project is an opportunity to understand not only how a pattern works, but also why it exists, when to apply it, and when a simpler solution would be better.