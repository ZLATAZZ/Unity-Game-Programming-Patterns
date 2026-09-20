using System;

namespace Patterns.Flyweight.Player
{
    public sealed class PlayerState
    {
        private int _currentHealth;
        private int _currentEnergy;
        private int _currentBlockAmount;

        public PlayerDefinition Definition { get; }

        public int CurrentHealth => _currentHealth;
        public int CurrentEnergy => _currentEnergy;
        public int CurrentBlockAmount => _currentBlockAmount;

        public int HandCapacity => Definition.HandCapacity;

        public bool IsDead => _currentHealth <= 0;

        public PlayerState(PlayerDefinition definition)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));

            _currentHealth = Definition.MaxHealth;
            _currentEnergy = Definition.MaxEnergy;
        }

        public void TakeDamage(int damage)
        {
            if (damage < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(damage));
            }

            int absorbedDamage = Math.Min(_currentBlockAmount, damage);

            _currentBlockAmount -= absorbedDamage;

            int healthDamage = damage - absorbedDamage;

            _currentHealth = Math.Max(0, _currentHealth - healthDamage);
        }

        public bool TrySpendEnergy(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }

            if (_currentEnergy < amount)
            {
                return false;
            }

            _currentEnergy -= amount;

            return true;
        }

        public void AddBlock(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }

            _currentBlockAmount += amount;
        }

        public void RestoreEnergy()
        {
            _currentEnergy = Definition.MaxEnergy;
        }

        public void ClearBlock()
        {
            _currentBlockAmount = 0;
        }
    }
}