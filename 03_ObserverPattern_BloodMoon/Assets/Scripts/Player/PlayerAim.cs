using System;
using BloodMoon.Input;
using UnityEngine;

namespace BloodMoon.Player
{
    public sealed class PlayerAim : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private LayerMask _groundLayer;

        private PlayerInputReader _inputReader;
        private Camera _camera;

        private bool _isInitialized;

        public void Initialize(PlayerInputReader inputReader, Camera camera)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(PlayerAim)} has already been initialized.");
            }

            _inputReader = inputReader ?? throw new ArgumentNullException(nameof(inputReader));
            _camera = camera ?? throw new ArgumentNullException(nameof(camera));

            _isInitialized = true;
        }

        private void Awake()
        {
            ValidateConfiguration();
        }

        private void FixedUpdate()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(PlayerAim)} has not been initialized.");
            }

            RotatePlayer();
        }

        private void RotatePlayer()
        {
            if (!TryGetLookPoint(out Vector3 lookPoint))
            {
                return;
            }

            Vector3 direction = lookPoint - _rigidbody.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.001f)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            _rigidbody.MoveRotation(targetRotation);
        }

        private bool TryGetLookPoint(out Vector3 lookPoint)
        {
            Ray ray = _camera.ScreenPointToRay(_inputReader.AimPosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _groundLayer))
            {
                lookPoint = hit.point;
                return true;
            }

            lookPoint = default;
            return false;
        }

        private void ValidateConfiguration()
        {
            if (_rigidbody == null)
            {
                throw new InvalidOperationException($"{nameof(PlayerAim)} requires a Rigidbody reference.");
            }

            if (_groundLayer.value == 0)
            {
                throw new InvalidOperationException($"{nameof(PlayerAim)} requires at least one Ground layer.");
            }
        }
    }
}