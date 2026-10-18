using System;
using CursedForest.Unity.Agents.Capabilities;
using UnityEngine;

namespace CursedForest.Unity.Combat
{
    [DisallowMultipleComponent]
    public sealed class DamageableHealth : MonoBehaviour, IDamageable
    {
        [SerializeField, Min(1f)] private float _maximumHealth = 100f;

        private float _currentHealth;

        public float CurrentHealth => _currentHealth;
        public float MaximumHealth => _maximumHealth;
        public bool IsAlive => _currentHealth > 0f;

        public event Action<Transform> Damaged;
        public event Action Died;

        private void Awake()
        {
            if (float.IsNaN(_maximumHealth) || float.IsInfinity(_maximumHealth) || _maximumHealth <= 0f)
            {
                throw new InvalidOperationException("Maximum health must be finite and positive.");
            }

            _currentHealth = _maximumHealth;
        }

        public void TakeDamage(float amount, Transform attacker)
        {
            if (float.IsNaN(amount) || float.IsInfinity(amount) || amount <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Damage must be finite and positive.");
            }

            if (!IsAlive)
            {
                return;
            }

            _currentHealth = Mathf.Max(0f, _currentHealth - amount);

            Damaged?.Invoke(attacker);

            if (!IsAlive)
            {
                Died?.Invoke();
            }
        }
    }
}