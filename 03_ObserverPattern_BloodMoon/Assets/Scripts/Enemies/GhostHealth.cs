using System;
using UnityEngine;

namespace BloodMoon.Enemies
{
    public sealed class GhostHealth : MonoBehaviour
    {
        [SerializeField, Min(1)] private int _maxHealth = 2;

        public int CurrentHealth { get; private set; }
        public int MaxHealth => _maxHealth;
        public bool IsDead { get; private set; }

        public event Action Died;

        private void Awake()
        {
            ValidateConfiguration();
            ResetHealth();
        }

        public void TakeDamage(int damage)
        {
            if (damage <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(damage), "Damage must be greater than zero.");
            }

            if (IsDead)
            {
                return;
            }

            CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

            if (CurrentHealth == 0)
            {
                Die();
            }
        }

        public void ResetHealth()
        {
            CurrentHealth = _maxHealth;
            IsDead = false;
        }

        private void Die()
        {
            IsDead = true;
            Died?.Invoke();
        }

        private void ValidateConfiguration()
        {
            if (_maxHealth <= 0)
            {
                throw new InvalidOperationException($"{nameof(GhostHealth)} max health must be greater than zero.");
            }
        }
    }
}