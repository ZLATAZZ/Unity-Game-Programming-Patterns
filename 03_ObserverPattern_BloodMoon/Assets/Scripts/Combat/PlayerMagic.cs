using System;
using BloodMoon.Core;
using BloodMoon.Input;
using BloodMoon.Presentation;
using UnityEngine;

namespace BloodMoon.Combat
{
    public sealed class PlayerMagic : MonoBehaviour, IBloodMoonObserver
    {
        [SerializeField] private PlayerMagicView _view;
        [SerializeField] private Transform _firePoint;
        [SerializeField, Min(0f)] private float _fireCooldown = 0.18f;

        private PlayerInputReader _inputReader;
        private ProjectileSpawner _projectileSpawner;

        private bool _isInitialized;
        private bool _isInputSubscribed;
        private bool _isMagicActive;

        private float _nextAllowedShotTime;

        public void Initialize(PlayerInputReader inputReader, ProjectileSpawner projectileSpawner, bool isBloodMoonActive)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(PlayerMagic)} has already been initialized.");
            }

            ValidateConfiguration();

            _inputReader = inputReader ?? throw new ArgumentNullException(nameof(inputReader));
            _projectileSpawner = projectileSpawner != null ? projectileSpawner : throw new ArgumentNullException(nameof(projectileSpawner));

            _isMagicActive = isBloodMoonActive;
            _view.SetVisibleImmediate(_isMagicActive);

            _isInitialized = true;

            if (isActiveAndEnabled)
            {
                SubscribeInput();
            }
        }

        private void OnEnable()
        {
            if (_isInitialized)
            {
                SubscribeInput();
            }
        }

        private void OnDisable()
        {
            UnsubscribeInput();
        }

        public void OnBloodMoonStarted()
        {
            EnsureInitialized();

            _isMagicActive = true;
            _nextAllowedShotTime = 0f;

            _view.Show();
        }

        public void OnBloodMoonEnded()
        {
            EnsureInitialized();

            _isMagicActive = false;

            _view.Hide();
        }

        private void HandleAttackPressed()
        {
            if (!_isMagicActive)
            {
                return;
            }

            if (Time.time < _nextAllowedShotTime)
            {
                return;
            }

            _nextAllowedShotTime = Time.time + _fireCooldown;

            _projectileSpawner.Fire(_firePoint.position, _firePoint.rotation);
        }

        private void SubscribeInput()
        {
            if (_isInputSubscribed)
            {
                return;
            }

            _inputReader.AttackPressed += HandleAttackPressed;
            _isInputSubscribed = true;
        }

        private void UnsubscribeInput()
        {
            if (!_isInputSubscribed)
            {
                return;
            }

            _inputReader.AttackPressed -= HandleAttackPressed;
            _isInputSubscribed = false;
        }

        private void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(PlayerMagic)} has not been initialized.");
            }
        }

        private void ValidateConfiguration()
        {
            if (_view == null)
            {
                throw new InvalidOperationException($"{nameof(PlayerMagic)} requires a PlayerMagicView reference.");
            }

            if (_firePoint == null)
            {
                throw new InvalidOperationException($"{nameof(PlayerMagic)} requires a Fire Point reference.");
            }

            if (_fireCooldown < 0f)
            {
                throw new InvalidOperationException($"{nameof(PlayerMagic)} fire cooldown cannot be negative.");
            }
        }
    }
}