# Flyweight Pattern - Witch Cards

https://github.com/user-attachments/assets/bd8f5292-c07a-4ca8-9b7c-d3ee877b4ede

A small Unity card example demonstrating the Flyweight pattern by separating shared card data from per-card runtime state.

Cards of the same type reference the same immutable definition instead of duplicating common data such as artwork, descriptions, and base properties.

## Demonstrates

- Flyweight pattern
- Intrinsic vs extrinsic state
- Shared immutable configuration
- Reduced duplication between repeated objects
- Separation of card definition data from runtime card state

## Architecture

Shared card information is stored once in a reusable definition.

Individual card instances reference that shared definition while keeping their own runtime state, such as position, selection state, or other instance-specific values.

## Purpose

The example shows when many objects can safely share common data instead of storing identical copies in every instance.