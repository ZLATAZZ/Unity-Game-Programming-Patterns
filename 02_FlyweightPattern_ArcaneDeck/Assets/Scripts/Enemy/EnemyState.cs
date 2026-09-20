using System;

namespace Patterns.Flyweight.Enemy
{
    public sealed class EnemyState
    {
        private int _currentHealth;
        private int _nextAttackDamage;

        public EnemyDefinition Definition { get; }
        public bool IsDead => _currentHealth <= 0;
        public int CurrentHealth => _currentHealth;
        public int NextAttackDamage => _nextAttackDamage;

        public EnemyState(EnemyDefinition definition)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));

            if (Definition.MaxHealth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(definition), "Enemy max health must be greater than zero.");
            }

            if (Definition.MinAttackDamage < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(definition), "Enemy minimum attack damage cannot be negative.");
            }

            if (Definition.MaxAttackDamage < Definition.MinAttackDamage)
            {
                throw new ArgumentException("Enemy maximum attack damage cannot be lower than minimum attack damage.", nameof(definition));
            }

            _currentHealth = Definition.MaxHealth;
        }

        public void TakeDamage(int damage)
        {
            if (damage < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(damage));
            }
            _currentHealth = Math.Max(0, _currentHealth - damage);
        }

        public void SetNextAttackDamage(int damage)
        {
            if (damage < Definition.MinAttackDamage || damage > Definition.MaxAttackDamage)
            {
                throw new ArgumentOutOfRangeException(nameof(damage));
            }
            _nextAttackDamage = damage;
        }
    }
}