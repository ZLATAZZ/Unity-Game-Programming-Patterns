using System;
using PrototypeForge.Core.Prototypes;

namespace PrototypeForge.Core.Enemies
{
    public sealed class EnemyPrototype : IPrototype<EnemyPrototype>
    {
        public PrototypeId Id { get; }
        public string DisplayName { get; }
        public int MaxHealth { get; }
        public float MoveSpeed { get; }
        public WeaponData Weapon { get; }

        public EnemyPrototype(PrototypeId id, string displayName, int maxHealth, float moveSpeed, WeaponData weapon)
        {
            if (id.IsEmpty)
            {
                throw new ArgumentException("Enemy prototype ID cannot be empty.", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Display name cannot be empty.", nameof(displayName));
            }

            if (maxHealth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxHealth), "Max health must be greater than zero.");
            }

            if (moveSpeed <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(moveSpeed), "Move speed must be greater than zero.");
            }

            Id = id;
            DisplayName = displayName;
            MaxHealth = maxHealth;
            MoveSpeed = moveSpeed;
            Weapon = weapon ?? throw new ArgumentNullException(nameof(weapon));
        }

        public EnemyPrototype Clone()
        {
            return new EnemyPrototype(
                Id,
                DisplayName,
                MaxHealth,
                MoveSpeed,
                Weapon.Clone());
        }
    }
}