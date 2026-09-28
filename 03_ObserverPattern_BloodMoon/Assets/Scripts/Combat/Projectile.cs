using System;
using BloodMoon.Enemies;
using BloodMoon.Pooling;
using UnityEngine;

namespace BloodMoon.Combat
{
    public sealed class Projectile : MonoBehaviour, IPoolable
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField, Min(0f)] private float _speed = 14f;
        [SerializeField, Min(0f)] private float _lifetime = 2f;
        [SerializeField, Min(1)] private int _damage = 1;

        private float _remainingLifetime;
        private bool _isActive;

        public event Action<Projectile> Finished;

        private void Awake()
        {
            ValidateConfiguration();
        }

        private void Update()
        {
            if (!_isActive)
            {
                return;
            }

            _remainingLifetime -= Time.deltaTime;

            if (_remainingLifetime <= 0f)
            {
                Finish();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isActive)
            {
                return;
            }

            Ghost ghost = other.GetComponentInParent<Ghost>();

            if (ghost == null)
            {
                return;
            }

            ghost.TakeDamage(_damage);
            Finish();
        }

        public void Launch(Vector3 direction)
        {
            if (!_isActive)
            {
                throw new InvalidOperationException($"{nameof(Projectile)} must be rented before it can be launched.");
            }

            if (direction.sqrMagnitude < 0.001f)
            {
                throw new ArgumentException("Projectile launch direction cannot be zero.", nameof(direction));
            }

            _rigidbody.linearVelocity = direction.normalized * _speed;
        }

        public void OnRent()
        {
            _remainingLifetime = _lifetime;
            _isActive = true;

            ResetPhysics();
        }

        public void OnReturn()
        {
            _isActive = false;
            _remainingLifetime = 0f;

            ResetPhysics();
        }

        private void Finish()
        {
            if (!_isActive)
            {
                return;
            }

            _isActive = false;
            Finished?.Invoke(this);
        }

        private void ResetPhysics()
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

        private void ValidateConfiguration()
        {
            if (_rigidbody == null)
            {
                throw new InvalidOperationException($"{nameof(Projectile)} requires a Rigidbody reference.");
            }

            if (_speed <= 0f)
            {
                throw new InvalidOperationException($"{nameof(Projectile)} speed must be greater than zero.");
            }

            if (_lifetime <= 0f)
            {
                throw new InvalidOperationException($"{nameof(Projectile)} lifetime must be greater than zero.");
            }

            if (_damage <= 0)
            {
                throw new InvalidOperationException($"{nameof(Projectile)} damage must be greater than zero.");
            }
        }
    }
}