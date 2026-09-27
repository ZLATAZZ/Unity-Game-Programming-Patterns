using System;
using BloodMoon.Input;
using BloodMoon.Player;
using UnityEngine;

namespace BloodMoon.Composition
{
    public sealed class GameBootstrapper : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private PlayerMovement _playerMovement;

        private PlayerInputReader _inputReader;

        private void Awake()
        {
            ValidateConfiguration();

            _inputReader = new PlayerInputReader();

            _playerMovement.Initialize(_inputReader);
        }

        private void OnEnable()
        {
            _inputReader?.Enable();
        }

        private void OnDisable()
        {
            _inputReader?.Disable();
        }

        private void OnDestroy()
        {
            _inputReader?.Dispose();
            _inputReader = null;
        }

        private void ValidateConfiguration()
        {
            if (_playerMovement == null)
            {
                throw new InvalidOperationException($"{nameof(GameBootstrapper)} requires a PlayerMovement reference.");
            }
        }
    }
}