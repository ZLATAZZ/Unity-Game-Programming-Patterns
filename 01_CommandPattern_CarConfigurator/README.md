# Command Pattern — Car Configurator

A small Unity project demonstrating the Command pattern through a car customization system.

Each customization change is represented as a command object rather than being applied directly. Commands can be executed, undone, and redone through a centralized command history.

## Demonstrates

- Command abstraction
- Encapsulation of reversible operations
- Undo / Redo with separate history stacks
- Separation between UI actions and customization logic
- Extensible customization features such as paint, wheels, and other car options

## Architecture

UI interaction creates a command describing the requested change.

The command applies the new customization state and stores enough information to restore the previous state when `Undo()` is called.

`CommandHistory` manages executed and reverted commands without depending on specific customization types.

## Purpose

The example focuses on replacing direct state mutation with explicit, reversible actions and keeping command execution independent from presentation code.