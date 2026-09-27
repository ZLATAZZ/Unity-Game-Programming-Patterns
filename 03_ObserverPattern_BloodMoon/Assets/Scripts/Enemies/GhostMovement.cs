using System;
using UnityEngine;

namespace BloodMoon.Enemies
{
    public sealed class GhostMovement : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField, Min(0f)] private float _speed = 2.5f;
        [SerializeField, Min(0f)] private float _stoppingDistance = 0.8f;

        private Transform _target;

        private void Awake()
        {
            ValidateConfiguration();
        }

        private void FixedUpdate()
        {
            if (_target == null)
            {
                return;
            }

            MoveTowardsTarget();
        }

        public void SetTarget(Transform target)
        {
            _target = target != null ? target : throw new ArgumentNullException(nameof(target));
        }

        public void ClearTarget()
        {
            _target = null;
        }

        public void ResetState()
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

        private void MoveTowardsTarget()
        {
            Vector3 direction = _target.position - _rigidbody.position;
            direction.y = 0f;

            float stoppingDistanceSquared = _stoppingDistance * _stoppingDistance;

            if (direction.sqrMagnitude <= stoppingDistanceSquared)
            {
                return;
            }

            direction.Normalize();

            Vector3 displacement = direction * _speed * Time.fixedDeltaTime;
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            _rigidbody.MovePosition(_rigidbody.position + displacement);
            _rigidbody.MoveRotation(targetRotation);
        }

        private void ValidateConfiguration()
        {
            if (_rigidbody == null)
            {
                throw new InvalidOperationException($"{nameof(GhostMovement)} requires a Rigidbody reference.");
            }

            if (_speed <= 0f)
            {
                throw new InvalidOperationException($"{nameof(GhostMovement)} speed must be greater than zero.");
            }
        }
    }
}