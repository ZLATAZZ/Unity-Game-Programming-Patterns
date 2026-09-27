using System;
using BloodMoon.Combat;
using BloodMoon.Core;
using BloodMoon.Enemies;
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
        [SerializeField] private PlayerMagic _playerMagic;

        [Header("Gameplay")]
        [SerializeField] private MoonAltar _moonAltar;
        [SerializeField] private ArenaBarrier _arenaBarrier;
        [SerializeField] private GhostSpawner _ghostSpawner;
        [SerializeField] private BloodMoonEnvironment _bloodMoonEnvironment;

        [Header("Combat")]
        [SerializeField] private ProjectileSpawner _projectileSpawner;

        private PlayerInputReader _inputReader;
        private BloodMoonSystem _bloodMoonSystem;
        private BloodMoonEncounterController _encounterController;

        private void Awake()
        {
            ValidateConfiguration();

            _inputReader = new PlayerInputReader();
            _bloodMoonSystem = new BloodMoonSystem();

            InitializeSystems();

            _encounterController = new BloodMoonEncounterController(_bloodMoonSystem, _ghostSpawner);

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

            _encounterController?.Dispose();
            _encounterController = null;

            _ghostSpawner?.Dispose();
            _projectileSpawner?.Dispose();

            _inputReader?.Dispose();
            _inputReader = null;
        }

        private void InitializeSystems()
        {
            _projectileSpawner.Initialize();
            _ghostSpawner.Initialize(_playerMovement.transform);

            _playerMovement.Initialize(_inputReader);
            _playerAim.Initialize(_inputReader, _mainCamera);
            _moonAltar.Initialize(_bloodMoonSystem, _inputReader);

            _arenaBarrier.Initialize(_bloodMoonSystem.IsActive);
            _playerMagic.Initialize(_inputReader, _projectileSpawner, _bloodMoonSystem.IsActive);
            _bloodMoonEnvironment.Initialize(_bloodMoonSystem.IsActive);
        }

        private void RegisterObservers()
        {
            _bloodMoonSystem.Subscribe(_arenaBarrier);
            _bloodMoonSystem.Subscribe(_ghostSpawner);
            _bloodMoonSystem.Subscribe(_playerMagic);
            _bloodMoonSystem.Subscribe(_bloodMoonEnvironment);
        }

        private void UnregisterObservers()
        {
            if (_bloodMoonSystem == null)
            {
                return;
            }

            _bloodMoonSystem.Unsubscribe(_arenaBarrier);
            _bloodMoonSystem.Unsubscribe(_ghostSpawner);
            _bloodMoonSystem.Unsubscribe(_playerMagic);
            _bloodMoonSystem.Unsubscribe(_bloodMoonEnvironment);
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

            if (_playerMagic == null)
            {
                throw new InvalidOperationException($"{nameof(GameBootstrapper)} requires a PlayerMagic reference.");
            }

            if (_moonAltar == null)
            {
                throw new InvalidOperationException($"{nameof(GameBootstrapper)} requires a MoonAltar reference.");
            }

            if (_arenaBarrier == null)
            {
                throw new InvalidOperationException($"{nameof(GameBootstrapper)} requires an ArenaBarrier reference.");
            }

            if (_ghostSpawner == null)
            {
                throw new InvalidOperationException($"{nameof(GameBootstrapper)} requires a GhostSpawner reference.");
            }

            if (_bloodMoonEnvironment == null)
            {
                throw new InvalidOperationException($"{nameof(GameBootstrapper)} requires a BloodMoonEnvironment reference.");
            }

            if (_projectileSpawner == null)
            {
                throw new InvalidOperationException($"{nameof(GameBootstrapper)} requires a ProjectileSpawner reference.");
            }
        }
    }
}