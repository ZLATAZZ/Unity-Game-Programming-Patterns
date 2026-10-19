using System;
using CursedForest.Unity.Agents.Capabilities;
using CursedForest.Unity.Combat;
using UnityEngine;

namespace CursedForest.Unity.Perception
{
   [DisallowMultipleComponent]
    public sealed class DamageReactionPerception : MonoBehaviour, IAgentPerception
    {
        [SerializeField] private DamageableHealth _observedHealth;
        [SerializeField, Min(0.1f)] private float _memorySeconds = 12f;

        private Transform _attacker;
        private float _lastDamageTime;

        public Transform CurrentTarget
        {
            get
            {
                if (_attacker == null || !_attacker.gameObject.activeInHierarchy)
                {
                    return null;
                }

                if (Time.time - _lastDamageTime > _memorySeconds)
                {
                    return null;
                }

                return _attacker;
            }
        }

        private void OnEnable()
        {
            if (_observedHealth == null)
            {
                Debug.LogError($"{name}: DamageReactionPerception requires Observed Health.", this);
                return;
            }

            _observedHealth.Damaged += HandleDamage;
            _observedHealth.Died += HandleObservedDeath;
        }

        private void OnDisable()
        {
            if (_observedHealth != null)
            {
                _observedHealth.Damaged -= HandleDamage;
                _observedHealth.Died -= HandleObservedDeath;
            }

            ClearTarget();
        }

        private void HandleDamage(Transform attacker)
        {
            if (attacker == null || attacker == transform || attacker.IsChildOf(transform))
            {
                return;
            }

            _attacker = attacker;
            _lastDamageTime = Time.time;
        }

        private void HandleObservedDeath()
        {
            ClearTarget();
        }

        private void ClearTarget()
        {
            _attacker = null;
            _lastDamageTime = 0f;
        }
    }
}
