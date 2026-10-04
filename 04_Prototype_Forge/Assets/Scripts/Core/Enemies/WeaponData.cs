using System;
using PrototypeForge.Core.Prototypes;

namespace PrototypeForge.Core.Enemies
{
    public sealed class WeaponData : IPrototype<WeaponData>
    {
        public PrototypeId Id { get; }
        public int Damage { get; private set; }
        public float Cooldown { get; }

        public WeaponData(PrototypeId id, int damage, float cooldown)
        {
            if (id.IsEmpty)
            {
                throw new ArgumentException("Weapon ID cannot be empty.", nameof(id));
            }

            if (damage <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(damage), "Damage must be greater than zero.");
            }

            if (cooldown <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(cooldown), "Cooldown must be greater than zero.");
            }

            Id = id;
            Damage = damage;
            Cooldown = cooldown;
        }

        public void IncreaseDamage(int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Damage increase must be greater than zero.");
            }

            Damage += amount;
        }

        public WeaponData Clone()
        {
            return new WeaponData(Id, Damage, Cooldown);
        }
    }
}