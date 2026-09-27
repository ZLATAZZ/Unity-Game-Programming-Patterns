using System;
using BloodMoon.Input;
using BloodMoon.Player;
using BloodMoon.Gameplay;
using BloodMoon.Core;
using UnityEngine;

namespace BloodMoon.Composition
{
    public sealed class GameBootstrapper : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private PlayerMovement _playerMovement;

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

        private void InitializeSystems()
        {
            _playerMovement.Initialize(_inputReader);
            _moonAltar.Initialize(_bloodMoonSystem, _inputReader);
            _arenaBarrier.Initialize(_bloodMoonSystem);
        }

        private void ValidateConfiguration()
        {
            if (_playerMovement == null)
            {
                throw new InvalidOperationException($"{nameof(GameBootstrapper)} requires a PlayerMovement reference.");
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