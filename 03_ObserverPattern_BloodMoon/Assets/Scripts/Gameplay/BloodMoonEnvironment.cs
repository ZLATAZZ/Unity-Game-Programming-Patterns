using System;
using BloodMoon.Core;
using BloodMoon.Presentation;
using UnityEngine;

namespace BloodMoon.Gameplay
{
    public sealed class BloodMoonEnvironment : MonoBehaviour, IBloodMoonObserver
    {
        [SerializeField] private BloodMoonEnvironmentView _view;

        private bool _isInitialized;

        public void Initialize(bool isBloodMoonActive)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(BloodMoonEnvironment)} has already been initialized.");
            }

            if (_view == null)
            {
                throw new InvalidOperationException($"{nameof(BloodMoonEnvironment)} requires a BloodMoonEnvironmentView reference.");
            }

            _view.SetBloodMoonImmediate(isBloodMoonActive);

            _isInitialized = true;
        }

        public void OnBloodMoonStarted()
        {
            EnsureInitialized();
            _view.PlayBloodMoon();
        }

        public void OnBloodMoonEnded()
        {
            EnsureInitialized();
            _view.PlayCalm();
        }

        private void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(BloodMoonEnvironment)} has not been initialized.");
            }
        }
    }
}