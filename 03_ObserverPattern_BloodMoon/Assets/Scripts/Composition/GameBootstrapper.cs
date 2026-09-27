using System;
using BloodMoon.Core;
using BloodMoon.Gameplay;
using BloodMoon.Input;
using BloodMoon.Player;
using UnityEngine;

namespace BloodMoon.Composition
{
    public sealed class GameBootstrapper : MonoBehaviour
    {
        [Header("Scene")]
        [SerializeField] private Camera _mainCamera;

        [Header("Player")]
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private PlayerAim _playerAim;

        [Header("Gameplay Systems")]
        [SerializeField] private MoonAltar _moonAltar;
        [SerializeField] private ArenaBarrier _arenaBarrier;

        private PlayerInputReader _inputReader;
        private BloodMoonSystem _bloodMoonSystem;

        private void Awake()
        {
            ValidateConfiguration();

            _inputReader = new PlayerInputReader();
            _bloodMoonSystem = new BloodMoonSystem();

            InitializeSystems();
            RegisterObservers();
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
            UnregisterObservers();

            _inputReader?.Dispose();
            _inputReader = null;
        }

        private void InitializeSystems()
        {
            _playerMovement.Initialize(_inputReader);
            _playerAim.Initialize(_inputReader, _mainCamera);
            _moonAltar.Initialize(_bloodMoonSystem, _inputReader);
        }

        private void RegisterObservers()
        {
            _bloodMoonSystem.Subscribe(_arenaBarrier);
        }

        private void UnregisterObservers()
        {
            _bloodMoonSystem?.Unsubscribe(_arenaBarrier);
        }

        private void ValidateConfiguration()
        {
            if (_mainCamera == null)
            {
                throw new InvalidOperationException($"{nameof(GameBootstrapper)} requires a Camera reference.");
            }

            if (_playerMovement == null)
            {
                throw new InvalidOperationException($"{nameof(GameBootstrapper)} requires a PlayerMovement reference.");
            }

            if (_playerAim == null)
            {
                throw new InvalidOperationException($"{nameof(GameBootstrapper)} requires a PlayerAim reference.");
            }

            if (_moonAltar == null)
            {
                throw new InvalidOperationException($"{nameof(GameBootstrapper)} requires a MoonAltar reference.");
            }

            if (_arenaBarrier == null)
            {
                throw new InvalidOperationException($"{nameof(GameBootstrapper)} requires an ArenaBarrier reference.");
            }
        }
    }
}