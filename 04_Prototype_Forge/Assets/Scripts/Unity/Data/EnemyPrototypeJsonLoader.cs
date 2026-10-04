using System;
using System.Collections.Generic;
using PrototypeForge.Core.Enemies;
using PrototypeForge.Core.Prototypes;
using UnityEngine;

namespace PrototypeForge.Unity.Data
{
    public sealed class EnemyPrototypeJsonLoader
    {
        public IReadOnlyList<EnemyPrototype> Load(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException("Prototype JSON cannot be empty.", nameof(json));
            }

            EnemyPrototypeCollectionDto root;

            try
            {
                root = JsonUtility.FromJson<EnemyPrototypeCollectionDto>(json);
            }
            catch (ArgumentException exception)
            {
                throw new InvalidOperationException("Failed to deserialize enemy prototype JSON.", exception);
            }

            if (root == null || root.prototypes == null || root.prototypes.Length == 0)
            {
                throw new InvalidOperationException("Prototype JSON must contain at least one enemy prototype.");
            }

            List<EnemyPrototype> prototypes = new(root.prototypes.Length);

            for (int i = 0; i < root.prototypes.Length; i++)
            {
                EnemyPrototypeDto dto = root.prototypes[i];

                Validate(dto, i);

                prototypes.Add(Map(dto));
            }

            return prototypes;
        }

        private static EnemyPrototype Map(EnemyPrototypeDto dto)
        {
            WeaponData weapon = new(
                new PrototypeId(dto.weapon.id),
                dto.weapon.damage,
                dto.weapon.cooldown);

            return new EnemyPrototype(
                new PrototypeId(dto.id),
                dto.displayName,
                dto.maxHealth,
                dto.moveSpeed,
                weapon);
        }

        private static void Validate(EnemyPrototypeDto dto, int index)
        {
            if (dto == null)
            {
                throw new InvalidOperationException($"Enemy prototype at index {index} is null.");
            }

            if (string.IsNullOrWhiteSpace(dto.id))
            {
                throw new InvalidOperationException($"Enemy prototype at index {index} has an empty ID.");
            }

            if (string.IsNullOrWhiteSpace(dto.displayName))
            {
                throw new InvalidOperationException($"Enemy prototype '{dto.id}' has an empty display name.");
            }

            if (dto.maxHealth <= 0)
            {
                throw new InvalidOperationException($"Enemy prototype '{dto.id}' must have positive max health.");
            }

            if (dto.moveSpeed <= 0f)
            {
                throw new InvalidOperationException($"Enemy prototype '{dto.id}' must have positive move speed.");
            }

            if (dto.weapon == null)
            {
                throw new InvalidOperationException($"Enemy prototype '{dto.id}' requires weapon data.");
            }

            if (string.IsNullOrWhiteSpace(dto.weapon.id))
            {
                throw new InvalidOperationException($"Enemy prototype '{dto.id}' contains a weapon with an empty ID.");
            }

            if (dto.weapon.damage <= 0)
            {
                throw new InvalidOperationException($"Enemy prototype '{dto.id}' weapon damage must be positive.");
            }

            if (dto.weapon.cooldown <= 0f)
            {
                throw new InvalidOperationException($"Enemy prototype '{dto.id}' weapon cooldown must be positive.");
            }
        }
    }
}