# Prototype Pattern - Monster Forge

https://github.com/user-attachments/assets/c9925ef6-071c-4eff-9c34-dfa56cb5e36c

A data-driven Unity project demonstrating the Prototype pattern by creating runtime monster configurations from existing configured prototypes.

Instead of reconstructing every monster configuration from scratch, original prototypes are registered once and fresh runtime copies are created through `Clone()`.

## Demonstrates

- Generic `IPrototype<T>` abstraction
- Generic `PrototypeRegistry<T>`
- Deep cloning of nested mutable objects
- Strongly typed prototype identifiers
- JSON-based gameplay configuration
- DTO-to-domain mapping and validation
- ScriptableObject catalog for Unity asset references
- Dynamic UI generated from prototype data
- Edit Mode tests
- Assembly Definition boundaries

## Deep Cloning

Each `EnemyPrototype` contains mutable `WeaponData`.

Cloning an enemy also clones its weapon data, so modifying one runtime clone does not affect the original prototype or other clones.

For example:

```text
Original Wisp     Damage: 8
Clone #1          Damage: 28
Clone #2          Damage: 8