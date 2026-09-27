using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BloodMoon.Input
{
    public sealed class PlayerInputReader : IDisposable
    {
        private readonly BloodMoonInputActions _inputActions;

        private bool _isEnabled;
        private bool _isDisposed;

        public Vector2 MoveInput => _inputActions.Player.Move.ReadValue<Vector2>();
        public Vector2 AimPosition => _inputActions.Player.Aim.ReadValue<Vector2>();

        public event Action InteractPressed;
        public event Action AttackPressed;

        public PlayerInputReader()
        {
            _inputActions = new BloodMoonInputActions();

            _inputActions.Player.Interact.performed += OnInteractPerformed;
            _inputActions.Player.Attack.performed += OnAttackPerformed;
        }

        public void Enable()
        {
            ThrowIfDisposed();

            if (_isEnabled)
            {
                return;
            }

            _inputActions.Player.Enable();
            _isEnabled = true;
        }

        public void Disable()
        {
            if (!_isEnabled)
            {
                return;
            }

            _inputActions.Player.Disable();
            _isEnabled = false;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            Disable();

            _inputActions.Player.Interact.performed -= OnInteractPerformed;
            _inputActions.Player.Attack.performed -= OnAttackPerformed;

            _inputActions.Dispose();

            _isDisposed = true;
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            InteractPressed?.Invoke();
        }

        private void OnAttackPerformed(InputAction.CallbackContext context)
        {
            AttackPressed?.Invoke();
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(PlayerInputReader));
            }
        }
    }
}