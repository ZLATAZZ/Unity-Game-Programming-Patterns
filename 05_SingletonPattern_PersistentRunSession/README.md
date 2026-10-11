# Singleton Pattern — Persistent Run Session

https://github.com/user-attachments/assets/c3c6e559-2b2c-46c9-b995-7fcb7acd0ac4

A Unity project demonstrating the Singleton pattern through a persistent game session shared across two scenes.

A single `RunSession` instance manages score and collected crystals, preserving progress when transitioning between Forest and Dungeon.

## Demonstrates

- Singleton implementation with a validated global access point
- Duplicate instance protection
- Persistent state using `DontDestroyOnLoad()`
- Encapsulated state modification
- Event-driven UI updates without polling
- Multiple consumers accessing the same Singleton
- ScriptableObject-based level configuration
- Conditional level progression

## How It Works

`RunSession.Instance` provides global access to the current game session.

The first instance persists across scene transitions, while duplicate instances are destroyed.

Different components read and modify the same session state. Level requirements determine when gameplay buttons are disabled and progression becomes available.

## Architectural Trade-offs

While Singleton provides convenient access to shared state, it also introduces **hidden dependencies, global mutable state, and tighter coupling**.

This implementation intentionally demonstrates these limitations. In larger projects, I generally prefer explicit dependency injection, which allows a single shared instance without requiring global access.

**Having one instance does not necessarily mean it should be globally accessible.**