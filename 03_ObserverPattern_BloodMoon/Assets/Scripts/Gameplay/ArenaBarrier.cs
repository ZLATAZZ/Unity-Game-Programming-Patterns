using System;
using BloodMoon.Core;
using BloodMoon.Presentation;
using UnityEngine;

namespace BloodMoon.Gameplay
{
    public sealed class ArenaBarrier : MonoBehaviour
    {
        [SerializeField] private ArenaBarrierView _view;

        private BloodMoonSystem _bloodMoonSystem;

        private bool _isInitialized;
        private bool _isSubscribed;

        public void Initialize(BloodMoonSystem bloodMoonSystem)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(ArenaBarrier)} has already been initialized.");
            }

            if (_view == null)
            {
                throw new InvalidOperationException($"{nameof(ArenaBarrier)} requires an ArenaBarrierView reference.");
            }

            _bloodMoonSystem = bloodMoonSystem ?? throw new ArgumentNullException(nameof(bloodMoonSystem));

            _isInitialized = true;

            SynchronizeState();

            if (isActiveAndEnabled)
            {
                Subscribe();
            }
        }

        private void OnEnable()
        {
            if (_isInitialized)
            {
                Subscribe();
            }
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void HandleBloodMoonStarted()
        {
            _view.Close();
        }

        private void HandleBloodMoonEnded()
        {
            _view.Open();
        }

        private void SynchronizeState()
        {
            if (_bloodMoonSystem.IsActive)
            {
                _view.CloseImmediate();
                return;
            }

            _view.OpenImmediate();
        }

        private void Subscribe()
        {
            if (_isSubscribed)
            {
                return;
            }

            _bloodMoonSystem.Started += HandleBloodMoonStarted;
            _bloodMoonSystem.Ended += HandleBloodMoonEnded;

            _isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_isSubscribed)
            {
                return;
            }

            _bloodMoonSystem.Started -= HandleBloodMoonStarted;
            _bloodMoonSystem.Ended -= HandleBloodMoonEnded;

            _isSubscribed = false;
        }
    }
}