# Observer Pattern - Blood Moon

https://github.com/user-attachments/assets/e26c11cd-e437-43be-bf69-aef8b5b99e70

A small Unity gameplay scenario demonstrating the Observer pattern through a Blood Moon state transition.

When the Blood Moon starts or ends, several independent gameplay and presentation systems react without the central subject depending on their concrete implementations.

## Demonstrates

- Explicit Subject and Observer roles
- `IBloodMoonObserver` abstraction
- Multiple independent concrete observers
- Explicit subscription and unsubscription
- Safe observer collection changes during notification
- Protection against reentrant state transitions
- Composition Root for dependency wiring

## Example Reactions

When the Blood Moon starts:

- the arena barrier closes;
- enemies spawn;
- player magic becomes available;
- environment lighting and visual effects change.

When the encounter ends, the same observers independently restore their normal state.

## Architecture

`BloodMoonSystem` acts as the Subject and stores observers through the `IBloodMoonObserver` interface.

Concrete systems such as `ArenaBarrier`, `GhostSpawner`, `PlayerMagic`, and `BloodMoonEnvironment` implement the observer contract.

Observer registration is performed explicitly in the application composition root, keeping the subject independent from concrete gameplay systems.

## Purpose

The example explores event-driven communication between loosely coupled systems while keeping ownership and dependencies explicit.