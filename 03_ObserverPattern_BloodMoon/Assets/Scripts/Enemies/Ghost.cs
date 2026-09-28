using System;
using BloodMoon.Pooling;
using BloodMoon.Presentation;
using UnityEngine;

namespace BloodMoon.Enemies
{
    public sealed class Ghost : MonoBehaviour, IPoolable
    {
        [SerializeField] private GhostHealth _health;
        [SerializeField] private GhostMovement _movement;
        [SerializeField] private GhostView _view;

        public event Action<Ghost> Died;

        private void Awake()
        {
            ValidateConfiguration();
            _health.Died += HandleDied;
        }

        private void OnDestroy()
        {
            if (_health != null)
            {
                _health.Died -= HandleDied;
            }
        }

        public void SetTarget(Transform target)
        {
            _movement.SetTarget(target);
        }

        public void TakeDamage(int damage)
        {
            if (_health.IsDead)
            {
                return;
            }

            _view.PlayHit();
            _health.TakeDamage(damage);
        }

        public void OnRent()
        {
            _health.ResetHealth();
            _movement.ResetState();
            _view.ResetImmediate();
        }

        public void OnReturn()
        {
            _movement.ClearTarget();
            _movement.ResetState();
            _view.ResetImmediate();
        }

        private void HandleDied()
        {
            Died?.Invoke(this);
        }

        private void ValidateConfiguration()
        {
            if (_health == null)
            {
                throw new InvalidOperationException($"{nameof(Ghost)} requires a GhostHealth reference.");
            }

            if (_movement == null)
            {
                throw new InvalidOperationException($"{nameof(Ghost)} requires a GhostMovement reference.");
            }

            if (_view == null)
            {
                throw new InvalidOperationException($"{nameof(Ghost)} requires a GhostView reference.");
            }
        }
    }
}