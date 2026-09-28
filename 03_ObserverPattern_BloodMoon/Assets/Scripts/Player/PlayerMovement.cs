using System;
using BloodMoon.Input;
using UnityEngine;

namespace BloodMoon.Player
{
    public sealed class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField, Min(0f)] private float _speed = 5f;

        private PlayerInputReader _inputReader;

        public void Initialize(PlayerInputReader inputReader)
        {
            _inputReader = inputReader ?? throw new ArgumentNullException(nameof(inputReader));
        }

        private void Awake()
        {
            if (_rigidbody == null)
            {
                throw new InvalidOperationException($"{nameof(PlayerMovement)} requires a Rigidbody reference.");
            }

            if (_speed <= 0f)
            {
                throw new InvalidOperationException($"{nameof(PlayerMovement)} speed must be greater than zero.");
            }
        }

        private void FixedUpdate()
        {
            if (_inputReader == null)
            {
                throw new InvalidOperationException($"{nameof(PlayerMovement)} has not been initialized.");
            }

            MovePlayer();
        }

        private void MovePlayer()
        {
            Vector3 movementDirection = GetMovementDirection();

            Vector3 displacement = movementDirection * _speed * Time.fixedDeltaTime;

            _rigidbody.MovePosition(_rigidbody.position + displacement);
        }

        private Vector3 GetMovementDirection()
        {
            Vector2 movementInput = _inputReader.MoveInput;

            Vector3 movementDirection = new(movementInput.x, 0f, movementInput.y);

            return Vector3.ClampMagnitude(movementDirection, 1f);
        }
    }
}