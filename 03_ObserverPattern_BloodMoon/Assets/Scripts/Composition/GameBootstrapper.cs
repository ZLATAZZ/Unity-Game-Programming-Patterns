using BloodMoon.Core;
using BloodMoon.Enemies;
using BloodMoon.Gameplay;
using BloodMoon.Input;
using BloodMoon.Player;
using BloodMoon.Combat;
using System;
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
        [SerializeField] private PlayerMagic _playerMagic;
        [SerializeField] private ProjectileSpawner _projectileSpawner;

        [Header("Enemies")]
        [SerializeField] private GhostSpawner _ghostSpawner;

        private PlayerInputReader _inputReader;
        private BloodMoonSystem _bloodMoonSystem;
        private BloodMoonEncounterController _encounterController;

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

            _encounterController?.Dispose();
            _encounterController = null;

            _ghostSpawner?.Dispose();

            _inputReader?.Dispose();
            _inputReader = null;
        }

        private void InitializeSystems()
        {
            _encounterController = new BloodMoonEncounterController(_bloodMoonSystem, _ghostSpawner);

            _playerMovement.Initialize(_inputReader);
            _playerAim.Initialize(_inputReader, _mainCamera);
            _moonAltar.Initialize(_bloodMoonSystem, _inputReader);
            _arenaBarrier.Initialize(_bloodMoonSystem.IsActive);
            _ghostSpawner.Initialize(_playerMovement.transform);
            _playerMagic.Initialize(_inputReader, _projectileSpawner, _bloodMoonSystem.IsActive);
            _projectileSpawner.Initialize();
        }

        private void RegisterObservers()
        {
            _bloodMoonSystem.Subscribe(_arenaBarrier);
            _bloodMoonSystem.Subscribe(_ghostSpawner);
            _bloodMoonSystem.Subscribe(_playerMagic);
        }

        private void UnregisterObservers()
        {
            _bloodMoonSystem?.Unsubscribe(_arenaBarrier);
            _bloodMoonSystem?.Unsubscribe(_ghostSpawner);
            _bloodMoonSystem?.Unsubscribe(_playerMagic);
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
            if (_ghostSpawner == null)
            {
                throw new InvalidOperationException($"{nameof(GameBootstrapper)} requires a GhostSpawner reference.");
            }
            if (_playerMagic == null)
            {
                throw new InvalidOperationException($"{nameof(GameBootstrapper)} requires a PlayerMagic reference.");
            }
            if (_projectileSpawner == null)
            {
                throw new InvalidOperationException($"{nameof(GameBootstrapper)} requires a ProjectileSpawner reference.");
            }
        }
    }
}