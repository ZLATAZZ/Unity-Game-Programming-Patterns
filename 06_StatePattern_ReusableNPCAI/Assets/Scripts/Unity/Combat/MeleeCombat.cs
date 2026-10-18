using CursedForest.Unity.Agents.Capabilities;
using UnityEngine;

namespace CursedForest.Unity.Combat
{
    [DisallowMultipleComponent]
    public sealed class MeleeCombat : MonoBehaviour, IAgentCombat
    {
        [SerializeField, Min(0.1f)] private float _attackRange = 2f;
        [SerializeField, Min(0.1f)] private float _damage = 20f;
        [SerializeField, Min(0.1f)] private float _attackCooldown = 1f;
        [SerializeField] private Transform _attackOrigin;

        private float _nextAttackTime;

        public bool IsInRange(Transform target)
        {
            if (target == null || !target.gameObject.activeInHierarchy || target.IsChildOf(transform))
            {
                return false;
            }

            Vector3 origin = _attackOrigin != null ? _attackOrigin.position : transform.position;
            Vector3 difference = target.position - origin;
            difference.y = 0f;

            return difference.sqrMagnitude <= _attackRange * _attackRange;
        }

        public void TryAttack(Transform target)
        {
            if (!IsInRange(target) || Time.time < _nextAttackTime)
            {
                return;
            }

            IDamageable damageable = target.GetComponentInParent<IDamageable>();

            if (damageable == null || !damageable.IsAlive)
            {
                return;
            }

            _nextAttackTime = Time.time + _attackCooldown;
            damageable.TakeDamage(_damage, transform);
        }
    }
}